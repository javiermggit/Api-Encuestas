using DynamicSurvey.Application.Common.Exceptions;
using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using DynamicSurvey.Domain.Entities;
using DynamicSurvey.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicSurvey.Infrastructure.Services;

public class SurveyAdminService : ISurveyAdminService
{
    private readonly AppDbContext _context;

    public SurveyAdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateSurveyAsync(CreateSurveyDto dto, CancellationToken cancellationToken)
    {
        var normalizedName = dto.Name.Trim();

        var exists = await _context.Surveys
            .AsNoTracking()
            .AnyAsync(x => x.Name == normalizedName && x.Version == dto.Version, cancellationToken);

        if (exists)
            throw new BusinessException("Ya existe una encuesta con el mismo nombre y versión.");

        var entity = new Survey
        {
            Name = normalizedName,
            Description = dto.Description?.Trim(),
            IsActive = dto.IsActive,
            Version = dto.Version,
            CreatedAt = DateTime.UtcNow
        };

        _context.Surveys.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<int> CreateSectionAsync(CreateSurveySectionDto dto, CancellationToken cancellationToken)
    {
        var surveyExists = await _context.Surveys
            .AsNoTracking()
            .AnyAsync(x => x.Id == dto.SurveyId, cancellationToken);

        if (!surveyExists)
            throw new NotFoundException("La encuesta no existe.");

        var duplicatedOrder = await _context.SurveySections
            .AsNoTracking()
            .AnyAsync(x => x.SurveyId == dto.SurveyId && x.DisplayOrder == dto.DisplayOrder, cancellationToken);

        if (duplicatedOrder)
            throw new BusinessException("Ya existe una sección con el mismo orden dentro de la encuesta.");

        var entity = new SurveySection
        {
            SurveyId = dto.SurveyId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.SurveySections.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<int> CreateQuestionAsync(CreateSurveyQuestionDto dto, CancellationToken cancellationToken)
    {
        var sectionExists = await _context.SurveySections
            .AsNoTracking()
            .AnyAsync(x => x.Id == dto.SectionId, cancellationToken);

        if (!sectionExists)
            throw new NotFoundException("La sección no existe.");

        var duplicatedCode = await _context.SurveyQuestions
            .AsNoTracking()
            .AnyAsync(x => x.SectionId == dto.SectionId && x.Code == dto.Code.Trim(), cancellationToken);

        if (duplicatedCode)
            throw new BusinessException("Ya existe una pregunta con el mismo código dentro de la sección.");

        var duplicatedOrder = await _context.SurveyQuestions
            .AsNoTracking()
            .AnyAsync(x => x.SectionId == dto.SectionId && x.DisplayOrder == dto.DisplayOrder, cancellationToken);

        if (duplicatedOrder)
            throw new BusinessException("Ya existe una pregunta con el mismo orden dentro de la sección.");

        var entity = new SurveyQuestion
        {
            SectionId = dto.SectionId,
            Code = dto.Code.Trim(),
            Text = dto.Text.Trim(),
            QuestionType = dto.QuestionType.Trim(),
            DisplayOrder = dto.DisplayOrder,
            IsRequired = dto.IsRequired,
            Placeholder = dto.Placeholder?.Trim(),
            HelpText = dto.HelpText?.Trim(),
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.SurveyQuestions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<int> CreateOptionAsync(CreateSurveyQuestionOptionDto dto, CancellationToken cancellationToken)
    {
        var questionExists = await _context.SurveyQuestions
            .AsNoTracking()
            .AnyAsync(x => x.Id == dto.QuestionId, cancellationToken);

        if (!questionExists)
            throw new NotFoundException("La pregunta no existe.");

        var duplicatedOrder = await _context.SurveyQuestionOptions
            .AsNoTracking()
            .AnyAsync(x => x.QuestionId == dto.QuestionId && x.DisplayOrder == dto.DisplayOrder, cancellationToken);

        if (duplicatedOrder)
            throw new BusinessException("Ya existe una opción con el mismo orden dentro de la pregunta.");

        var entity = new SurveyQuestionOption
        {
            QuestionId = dto.QuestionId,
            Value = dto.Value.Trim(),
            Label = dto.Label.Trim(),
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive
        };

        _context.SurveyQuestionOptions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<List<SurveyRuleListItemDto>> GetRulesBySurveyAsync(int surveyId, CancellationToken cancellationToken)
    {
        return await _context.SurveyRules
            .AsNoTracking()
            .Where(x => x.SurveyId == surveyId)
            .OrderBy(x => x.Priority)
            .Include(x => x.Conditions)
            .Include(x => x.Actions)
            .Select(x => new SurveyRuleListItemDto
            {
                Id = x.Id,
                SurveyId = x.SurveyId,
                Name = x.Name,
                Description = x.Description,
                Priority = x.Priority,
                StopProcessing = x.StopProcessing,
                IsActive = x.IsActive,
                Conditions = x.Conditions
                    .OrderBy(c => c.LogicalGroup)
                    .ThenBy(c => c.Id)
                    .Select(c => new SurveyRuleConditionSummaryDto
                    {
                        Id = c.Id,
                        QuestionId = c.QuestionId,
                        Operator = c.Operator,
                        ExpectedValue = c.ExpectedValue,
                        LogicalGroup = c.LogicalGroup
                    })
                    .ToList(),
                Actions = x.Actions
                    .OrderBy(a => a.OrderIndex)
                    .Select(a => new SurveyRuleActionSummaryDto
                    {
                        Id = a.Id,
                        ActionType = a.ActionType,
                        TargetQuestionId = a.TargetQuestionId,
                        TargetSectionId = a.TargetSectionId,
                        OrderIndex = a.OrderIndex
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<SurveyAdminDetailDto?> GetSurveyByIdAsync(int surveyId, CancellationToken cancellationToken)
    {
        var survey = await _context.Surveys
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Sections.OrderBy(s => s.DisplayOrder))
                .ThenInclude(s => s.Questions.OrderBy(q => q.DisplayOrder))
                    .ThenInclude(q => q.Options.OrderBy(o => o.DisplayOrder))
            .FirstOrDefaultAsync(x => x.Id == surveyId, cancellationToken);

        if (survey is null)
            return null;

        return new SurveyAdminDetailDto
        {
            Id = survey.Id,
            Name = survey.Name,
            Description = survey.Description,
            IsActive = survey.IsActive,
            Version = survey.Version,
            CreatedAt = survey.CreatedAt,
            UpdatedAt = survey.UpdatedAt,
            Sections = survey.Sections
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new SurveySectionDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    DisplayOrder = s.DisplayOrder,
                    Questions = s.Questions
                        .OrderBy(q => q.DisplayOrder)
                        .Select(q => new SurveyQuestionDto
                        {
                            Id = q.Id,
                            Code = q.Code,
                            Text = q.Text,
                            QuestionType = q.QuestionType,
                            DisplayOrder = q.DisplayOrder,
                            IsRequired = q.IsRequired,
                            Placeholder = q.Placeholder,
                            HelpText = q.HelpText,
                            Options = q.Options
                                .OrderBy(o => o.DisplayOrder)
                                .Select(o => new SurveyQuestionOptionDto
                                {
                                    Id = o.Id,
                                    Value = o.Value,
                                    Label = o.Label,
                                    DisplayOrder = o.DisplayOrder
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList()
        };
    }

    public async Task<List<SurveyQuestionDto>> GetQuestionsBySectionAsync(int sectionId, CancellationToken cancellationToken)
    {
        var sectionExists = await _context.SurveySections
            .AsNoTracking()
            .AnyAsync(x => x.Id == sectionId, cancellationToken);

        if (!sectionExists)
            throw new NotFoundException("La sección no existe.");

        return await _context.SurveyQuestions
            .AsNoTracking()
            .Where(x => x.SectionId == sectionId)
            .OrderBy(x => x.DisplayOrder)
            .Select(q => new SurveyQuestionDto
            {
                Id = q.Id,
                Code = q.Code,
                Text = q.Text,
                QuestionType = q.QuestionType,
                DisplayOrder = q.DisplayOrder,
                IsRequired = q.IsRequired,
                Placeholder = q.Placeholder,
                HelpText = q.HelpText,
                Options = q.Options
                    .Where(o => o.IsActive)
                    .OrderBy(o => o.DisplayOrder)
                    .Select(o => new SurveyQuestionOptionDto
                    {
                        Id = o.Id,
                        Value = o.Value,
                        Label = o.Label,
                        DisplayOrder = o.DisplayOrder
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SurveyQuestionOptionDto>> GetOptionsByQuestionAsync(int questionId, CancellationToken cancellationToken)
    {
        var questionExists = await _context.SurveyQuestions
            .AsNoTracking()
            .AnyAsync(x => x.Id == questionId, cancellationToken);

        if (!questionExists)
            throw new NotFoundException("La pregunta no existe.");

        return await _context.SurveyQuestionOptions
            .AsNoTracking()
            .Where(x => x.QuestionId == questionId)
            .OrderBy(x => x.DisplayOrder)
            .Select(o => new SurveyQuestionOptionDto
            {
                Id = o.Id,
                Value = o.Value,
                Label = o.Label,
                DisplayOrder = o.DisplayOrder
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SurveyListItemDto>> GetSurveysAsync(CancellationToken cancellationToken)
    {
        return await _context.Surveys
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new SurveyListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                Version = x.Version,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UpdateSurveyAsync(int surveyId, UpdateSurveyDto dto, CancellationToken cancellationToken)
    {
        var survey = await _context.Surveys
            .FirstOrDefaultAsync(x => x.Id == surveyId, cancellationToken);

        if (survey is null)
            return false;

        var normalizedName = dto.Name.Trim();

        var duplicated = await _context.Surveys
            .AsNoTracking()
            .AnyAsync(x => x.Id != surveyId && x.Name == normalizedName && x.Version == dto.Version, cancellationToken);

        if (duplicated)
            throw new BusinessException("Ya existe otra encuesta con el mismo nombre y versión.");

        survey.Name = normalizedName;
        survey.Description = dto.Description?.Trim();
        survey.IsActive = dto.IsActive;
        survey.Version = dto.Version;
        survey.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
