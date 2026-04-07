using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using DynamicSurvey.Domain.Entities;
using DynamicSurvey.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicSurvey.Infrastructure.Services;

public class SurveySessionService : ISurveySessionService
{
    private readonly AppDbContext _context;
    private readonly ISurveyRuleEngine _ruleEngine;

    public SurveySessionService(AppDbContext context, ISurveyRuleEngine ruleEngine)
    {
        _context = context;
        _ruleEngine = ruleEngine;
    }

    public async Task<long> CreateSessionAsync(CreateSessionDto dto, CancellationToken cancellationToken)
    {
        var firstSection = await _context.SurveySections
            .Where(x => x.SurveyId == dto.SurveyId && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .FirstOrDefaultAsync(cancellationToken);

        if (firstSection is null)
            throw new Exception("La encuesta no tiene secciones activas.");

        var firstQuestion = await _context.SurveyQuestions
            .Where(x => x.SectionId == firstSection.Id && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .FirstOrDefaultAsync(cancellationToken);

        var session = new SurveySession
        {
            SurveyId = dto.SurveyId,
            UserId = dto.UserId,
            Status = "InProgress",
            CurrentSectionId = firstSection.Id,
            CurrentQuestionId = firstQuestion?.Id,
            StartedAt = DateTime.UtcNow
        };

        _context.SurveySessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        return session.Id;
    }

    public async Task<SurveySessionDetailDto?> GetByIdAsync(long sessionId, CancellationToken cancellationToken)
    {
        var session = await _context.SurveySessions
            .AsNoTracking()
            .Include(x => x.Survey)
            .Include(x => x.Answers)
                .ThenInclude(a => a.Question)
            .Include(x => x.Answers)
                .ThenInclude(a => a.AnswerOptions)
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);

        if (session is null)
            return null;

        string? currentSectionTitle = null;
        string? currentQuestionText = null;

        if (session.CurrentSectionId.HasValue)
        {
            var currentSection = await _context.SurveySections
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == session.CurrentSectionId.Value, cancellationToken);

            currentSectionTitle = currentSection?.Title;
        }

        if (session.CurrentQuestionId.HasValue)
        {
            var currentQuestion = await _context.SurveyQuestions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == session.CurrentQuestionId.Value, cancellationToken);

            currentQuestionText = currentQuestion?.Text;
        }

        return new SurveySessionDetailDto
        {
            Id = session.Id,
            SurveyId = session.SurveyId,
            SurveyName = session.Survey?.Name,
            UserId = session.UserId,
            Status = session.Status,
            CurrentSectionId = session.CurrentSectionId,
            CurrentSectionTitle = currentSectionTitle,
            CurrentQuestionId = session.CurrentQuestionId,
            CurrentQuestionText = currentQuestionText,
            StartedAt = session.StartedAt,
            CompletedAt = session.CompletedAt,
            Answers = session.Answers
                .OrderBy(a => a.QuestionId)
                .Select(a => new SurveySessionAnswerDto
                {
                    AnswerId = a.Id,
                    QuestionId = a.QuestionId,
                    QuestionCode = a.Question?.Code,
                    QuestionText = a.Question?.Text,
                    AnswerValue = a.AnswerValue,
                    OptionIds = a.AnswerOptions.Select(o => o.OptionId).ToList()
                })
                .ToList()
        };
    }

    public async Task<NavigationResultDto> SaveAnswerAndNavigateAsync(long sessionId, SaveAnswerDto dto, CancellationToken cancellationToken)
    {
        var session = await _context.SurveySessions
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);

        if (session is null)
            throw new Exception("La sesión no existe.");

        var existingAnswer = await _context.SurveyAnswers
            .Include(x => x.AnswerOptions)
            .FirstOrDefaultAsync(x => x.SessionId == sessionId && x.QuestionId == dto.QuestionId, cancellationToken);

        if (existingAnswer is null)
        {
            existingAnswer = new SurveyAnswer
            {
                SessionId = sessionId,
                QuestionId = dto.QuestionId,
                AnswerValue = dto.AnswerValue,
                CreatedAt = DateTime.UtcNow
            };

            _context.SurveyAnswers.Add(existingAnswer);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            existingAnswer.AnswerValue = dto.AnswerValue;
            existingAnswer.UpdatedAt = DateTime.UtcNow;

            _context.SurveyAnswerOptions.RemoveRange(existingAnswer.AnswerOptions);
            await _context.SaveChangesAsync(cancellationToken);
        }

        if (dto.OptionIds is not null && dto.OptionIds.Count > 0)
        {
            foreach (var optionId in dto.OptionIds)
            {
                _context.SurveyAnswerOptions.Add(new SurveyAnswerOption
                {
                    AnswerId = existingAnswer.Id,
                    OptionId = optionId
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        var navigation = await _ruleEngine.EvaluateAsync(
            session.SurveyId,
            dto.QuestionId,
            dto.AnswerValue,
            cancellationToken);

        if (navigation.EndSurvey)
        {
            session.Status = "Completed";
            session.CompletedAt = DateTime.UtcNow;
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return navigation;
        }

        if (navigation.NextQuestionId.HasValue)
        {
            var nextQuestion = await _context.SurveyQuestions
                .Include(q => q.Section)
                .FirstOrDefaultAsync(q => q.Id == navigation.NextQuestionId.Value, cancellationToken);

            session.CurrentQuestionId = nextQuestion?.Id;
            session.CurrentSectionId = nextQuestion?.SectionId;
            session.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return navigation;
        }

        if (navigation.NextSectionId.HasValue)
        {
            session.CurrentSectionId = navigation.NextSectionId.Value;

            var nextQuestion = await _context.SurveyQuestions
                .Where(x => x.SectionId == navigation.NextSectionId.Value && x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .FirstOrDefaultAsync(cancellationToken);

            session.CurrentQuestionId = nextQuestion?.Id;
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            navigation.NextQuestionId = nextQuestion?.Id;
            return navigation;
        }

        var currentQuestion = await _context.SurveyQuestions
            .Include(x => x.Section)
            .FirstOrDefaultAsync(x => x.Id == dto.QuestionId, cancellationToken);

        if (currentQuestion is null)
            throw new Exception("La pregunta actual no existe.");

        var nextNormalQuestion = await _context.SurveyQuestions
            .Where(x => x.SectionId == currentQuestion.SectionId &&
                        x.DisplayOrder > currentQuestion.DisplayOrder &&
                        x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .FirstOrDefaultAsync(cancellationToken);

        if (nextNormalQuestion is not null)
        {
            session.CurrentQuestionId = nextNormalQuestion.Id;
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return new NavigationResultDto
            {
                NextQuestionId = nextNormalQuestion.Id
            };
        }

        var nextSection = await _context.SurveySections
            .Where(x => x.SurveyId == session.SurveyId &&
                        x.DisplayOrder > currentQuestion.Section.DisplayOrder &&
                        x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .FirstOrDefaultAsync(cancellationToken);

        if (nextSection is not null)
        {
            var firstQuestionOfNextSection = await _context.SurveyQuestions
                .Where(x => x.SectionId == nextSection.Id && x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .FirstOrDefaultAsync(cancellationToken);

            session.CurrentSectionId = nextSection.Id;
            session.CurrentQuestionId = firstQuestionOfNextSection?.Id;
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return new NavigationResultDto
            {
                NextSectionId = nextSection.Id,
                NextQuestionId = firstQuestionOfNextSection?.Id
            };
        }

        session.Status = "Completed";
        session.CompletedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return new NavigationResultDto
        {
            EndSurvey = true
        };
    }
}