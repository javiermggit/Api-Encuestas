using DynamicSurvey.Application.Common.Exceptions;
using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using DynamicSurvey.Domain.Entities;
using DynamicSurvey.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicSurvey.Infrastructure.Services;

public class SurveyRuleService : ISurveyRuleService
{
    private static readonly HashSet<string> SupportedOperators = new(StringComparer.OrdinalIgnoreCase)
    {
        "Equals", "NotEquals", "Contains", "IsEmpty", "IsNotEmpty"
    };

    private static readonly HashSet<string> SupportedActions = new(StringComparer.OrdinalIgnoreCase)
    {
        "GoToQuestion", "GoToSection", "ShowQuestion", "HideQuestion", "SetRequired", "EndSurvey"
    };

    private readonly AppDbContext _context;

    public SurveyRuleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateRuleAsync(CreateSurveyRuleDto dto, CancellationToken cancellationToken)
    {
        var surveyExists = await _context.Surveys
            .AsNoTracking()
            .AnyAsync(x => x.Id == dto.SurveyId, cancellationToken);

        if (!surveyExists)
            throw new NotFoundException("La encuesta no existe.");

        if (dto.Conditions.Count == 0)
            throw new BusinessException("La regla debe tener al menos una condición.");

        if (dto.Actions.Count == 0)
            throw new BusinessException("La regla debe tener al menos una acción.");

        var surveyQuestionIds = await _context.SurveyQuestions
            .AsNoTracking()
            .Where(x => x.Section.SurveyId == dto.SurveyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var surveySectionIds = await _context.SurveySections
            .AsNoTracking()
            .Where(x => x.SurveyId == dto.SurveyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var validQuestionIds = surveyQuestionIds.ToHashSet();
        var validSectionIds = surveySectionIds.ToHashSet();

        foreach (var condition in dto.Conditions)
        {
            if (!validQuestionIds.Contains(condition.QuestionId))
                throw new BusinessException($"La pregunta {condition.QuestionId} no pertenece a la encuesta.");

            if (!SupportedOperators.Contains(condition.Operator))
                throw new BusinessException($"El operador '{condition.Operator}' no es válido.");
        }

        foreach (var action in dto.Actions)
        {
            if (!SupportedActions.Contains(action.ActionType))
                throw new BusinessException($"La acción '{action.ActionType}' no es válida.");

            if (action.TargetQuestionId.HasValue && !validQuestionIds.Contains(action.TargetQuestionId.Value))
                throw new BusinessException($"La pregunta destino {action.TargetQuestionId.Value} no pertenece a la encuesta.");

            if (action.TargetSectionId.HasValue && !validSectionIds.Contains(action.TargetSectionId.Value))
                throw new BusinessException($"La sección destino {action.TargetSectionId.Value} no pertenece a la encuesta.");
        }

        var rule = new SurveyRule
        {
            SurveyId = dto.SurveyId,
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            Priority = dto.Priority,
            StopProcessing = dto.StopProcessing,
            IsActive = true,
            Conditions = dto.Conditions.Select(x => new SurveyRuleCondition
            {
                QuestionId = x.QuestionId,
                Operator = x.Operator.Trim(),
                ExpectedValue = x.ExpectedValue?.Trim(),
                LogicalGroup = x.LogicalGroup,
                IsActive = true
            }).ToList(),
            Actions = dto.Actions.Select(x => new SurveyRuleAction
            {
                ActionType = x.ActionType.Trim(),
                TargetQuestionId = x.TargetQuestionId,
                TargetSectionId = x.TargetSectionId,
                OrderIndex = x.OrderIndex,
                IsActive = true
            }).ToList()
        };

        _context.SurveyRules.Add(rule);
        await _context.SaveChangesAsync(cancellationToken);
        return rule.Id;
    }
}
