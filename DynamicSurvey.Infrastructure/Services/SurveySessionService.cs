using DynamicSurvey.Application.Common.Exceptions;
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
        var surveyExists = await _context.Surveys
            .AsNoTracking()
            .AnyAsync(x => x.Id == dto.SurveyId && x.IsActive, cancellationToken);

        if (!surveyExists)
            throw new NotFoundException("La encuesta no existe o está inactiva.");

        var firstSection = await _context.SurveySections
            .AsNoTracking()
            .Where(x => x.SurveyId == dto.SurveyId && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .FirstOrDefaultAsync(cancellationToken);

        if (firstSection is null)
            throw new BusinessException("La encuesta no tiene secciones activas.");

        var firstQuestion = await _context.SurveyQuestions
            .AsNoTracking()
            .Where(x => x.SectionId == firstSection.Id && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .FirstOrDefaultAsync(cancellationToken);

        if (firstQuestion is null)
            throw new BusinessException("La primera sección no tiene preguntas activas.");

        var session = new SurveySession
        {
            SurveyId = dto.SurveyId,
            UserId = dto.UserId?.Trim(),
            Status = "InProgress",
            CurrentSectionId = firstSection.Id,
            CurrentQuestionId = firstQuestion.Id,
            StartedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
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
            currentSectionTitle = await _context.SurveySections
                .AsNoTracking()
                .Where(x => x.Id == session.CurrentSectionId.Value)
                .Select(x => x.Title)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (session.CurrentQuestionId.HasValue)
        {
            currentQuestionText = await _context.SurveyQuestions
                .AsNoTracking()
                .Where(x => x.Id == session.CurrentQuestionId.Value)
                .Select(x => x.Text)
                .FirstOrDefaultAsync(cancellationToken);
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

    public async Task<SessionProgressDto?> GetProgressAsync(long sessionId, CancellationToken cancellationToken)
    {
        var session = await _context.SurveySessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);

        if (session is null)
            return null;

        var totalQuestions = await _context.SurveyQuestions
            .AsNoTracking()
            .CountAsync(x => x.IsActive && x.Section.SurveyId == session.SurveyId, cancellationToken);

        var answeredQuestions = await _context.SurveyAnswers
            .AsNoTracking()
            .CountAsync(x => x.SessionId == sessionId, cancellationToken);

        string? currentSectionTitle = null;
        string? currentQuestionText = null;

        if (session.CurrentSectionId.HasValue)
        {
            currentSectionTitle = await _context.SurveySections
                .AsNoTracking()
                .Where(x => x.Id == session.CurrentSectionId.Value)
                .Select(x => x.Title)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (session.CurrentQuestionId.HasValue)
        {
            currentQuestionText = await _context.SurveyQuestions
                .AsNoTracking()
                .Where(x => x.Id == session.CurrentQuestionId.Value)
                .Select(x => x.Text)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var completionPercentage = totalQuestions == 0
            ? 0
            : Math.Round((decimal)answeredQuestions * 100 / totalQuestions, 2);

        return new SessionProgressDto
        {
            SessionId = session.Id,
            SurveyId = session.SurveyId,
            Status = session.Status,
            TotalQuestions = totalQuestions,
            AnsweredQuestions = answeredQuestions,
            CompletionPercentage = completionPercentage,
            CurrentSectionId = session.CurrentSectionId,
            CurrentSectionTitle = currentSectionTitle,
            CurrentQuestionId = session.CurrentQuestionId,
            CurrentQuestionText = currentQuestionText
        };
    }

    public async Task<bool> CompleteSessionAsync(long sessionId, CancellationToken cancellationToken)
    {
        var session = await _context.SurveySessions
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);

        if (session is null)
            return false;

        if (session.Status == "Completed")
            return true;

        session.Status = "Completed";
        session.CompletedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<NavigationResultDto> SaveAnswerAndNavigateAsync(long sessionId, SaveAnswerDto dto, CancellationToken cancellationToken)
    {
        var session = await _context.SurveySessions
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);

        if (session is null)
            throw new NotFoundException("La sesión no existe.");

        if (session.Status == "Completed")
            throw new BusinessException("La sesión ya está finalizada.");

        var question = await _context.SurveyQuestions
            .AsNoTracking()
            .Include(x => x.Section)
            .FirstOrDefaultAsync(x => x.Id == dto.QuestionId && x.Section.SurveyId == session.SurveyId, cancellationToken);

        if (question is null)
            throw new NotFoundException("La pregunta no existe en la encuesta de la sesión.");

        var optionIds = dto.OptionIds?
            .Where(x => x > 0)
            .Distinct()
            .ToList() ?? new List<int>();

        if (question.IsRequired && string.IsNullOrWhiteSpace(dto.AnswerValue) && optionIds.Count == 0)
            throw new BusinessException("La pregunta es obligatoria.");

        if (optionIds.Count > 0)
        {
            var validOptionIds = await _context.SurveyQuestionOptions
                .AsNoTracking()
                .Where(x => x.QuestionId == dto.QuestionId && optionIds.Contains(x.Id) && x.IsActive)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            if (validOptionIds.Count != optionIds.Count)
                throw new BusinessException("Una o más opciones no pertenecen a la pregunta.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var existingAnswer = await _context.SurveyAnswers
            .Include(x => x.AnswerOptions)
            .FirstOrDefaultAsync(x => x.SessionId == sessionId && x.QuestionId == dto.QuestionId, cancellationToken);

        if (existingAnswer is null)
        {
            existingAnswer = new SurveyAnswer
            {
                SessionId = sessionId,
                QuestionId = dto.QuestionId,
                AnswerValue = dto.AnswerValue?.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.SurveyAnswers.Add(existingAnswer);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            existingAnswer.AnswerValue = dto.AnswerValue?.Trim();
            existingAnswer.UpdatedAt = DateTime.UtcNow;

            if (existingAnswer.AnswerOptions.Count > 0)
                _context.SurveyAnswerOptions.RemoveRange(existingAnswer.AnswerOptions);

            await _context.SaveChangesAsync(cancellationToken);
        }

        if (optionIds.Count > 0)
        {
            foreach (var optionId in optionIds)
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
            sessionId,
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
            await transaction.CommitAsync(cancellationToken);
            return navigation;
        }

        if (navigation.NextQuestionId.HasValue)
        {
            var nextQuestion = await _context.SurveyQuestions
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == navigation.NextQuestionId.Value && q.IsActive, cancellationToken);

            if (nextQuestion is null)
                throw new BusinessException("La navegación apunta a una pregunta inexistente o inactiva.");

            session.CurrentQuestionId = nextQuestion.Id;
            session.CurrentSectionId = nextQuestion.SectionId;
            session.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return navigation;
        }

        if (navigation.NextSectionId.HasValue)
        {
            var nextQuestion = await _context.SurveyQuestions
                .AsNoTracking()
                .Where(x => x.SectionId == navigation.NextSectionId.Value && x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .FirstOrDefaultAsync(cancellationToken);

            if (nextQuestion is null)
                throw new BusinessException("La sección destino no tiene preguntas activas.");

            session.CurrentSectionId = navigation.NextSectionId.Value;
            session.CurrentQuestionId = nextQuestion.Id;
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            navigation.NextQuestionId = nextQuestion.Id;
            await transaction.CommitAsync(cancellationToken);
            return navigation;
        }

        var nextNormalQuestion = await _context.SurveyQuestions
            .AsNoTracking()
            .Where(x => x.SectionId == question.SectionId && x.DisplayOrder > question.DisplayOrder && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .FirstOrDefaultAsync(cancellationToken);

        if (nextNormalQuestion is not null)
        {
            session.CurrentQuestionId = nextNormalQuestion.Id;
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new NavigationResultDto
            {
                NextQuestionId = nextNormalQuestion.Id
            };
        }

        var nextSection = await _context.SurveySections
            .AsNoTracking()
            .Where(x => x.SurveyId == session.SurveyId && x.DisplayOrder > question.Section.DisplayOrder && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .FirstOrDefaultAsync(cancellationToken);

        if (nextSection is not null)
        {
            var firstQuestionOfNextSection = await _context.SurveyQuestions
                .AsNoTracking()
                .Where(x => x.SectionId == nextSection.Id && x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .FirstOrDefaultAsync(cancellationToken);

            if (firstQuestionOfNextSection is null)
                throw new BusinessException("La siguiente sección no tiene preguntas activas.");

            session.CurrentSectionId = nextSection.Id;
            session.CurrentQuestionId = firstQuestionOfNextSection.Id;
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new NavigationResultDto
            {
                NextSectionId = nextSection.Id,
                NextQuestionId = firstQuestionOfNextSection.Id
            };
        }

        session.Status = "Completed";
        session.CompletedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new NavigationResultDto
        {
            EndSurvey = true
        };
    }
}
