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
        var entity = new Survey
        {
            Name = dto.Name,
            Description = dto.Description,
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
        var entity = new SurveySection
        {
            SurveyId = dto.SurveyId,
            Title = dto.Title,
            Description = dto.Description,
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
        var entity = new SurveyQuestion
        {
            SectionId = dto.SectionId,
            Code = dto.Code,
            Text = dto.Text,
            QuestionType = dto.QuestionType,
            DisplayOrder = dto.DisplayOrder,
            IsRequired = dto.IsRequired,
            Placeholder = dto.Placeholder,
            HelpText = dto.HelpText,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.SurveyQuestions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<int> CreateOptionAsync(CreateSurveyQuestionOptionDto dto, CancellationToken cancellationToken)
    {
        var entity = new SurveyQuestionOption
        {
            QuestionId = dto.QuestionId,
            Value = dto.Value,
            Label = dto.Label,
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
                    .OrderBy(c => c.Id)
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

        survey.Name = dto.Name;
        survey.Description = dto.Description;
        survey.IsActive = dto.IsActive;
        survey.Version = dto.Version;
        survey.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}