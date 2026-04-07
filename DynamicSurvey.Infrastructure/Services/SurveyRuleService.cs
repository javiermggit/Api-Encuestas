using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using DynamicSurvey.Domain.Entities;
using DynamicSurvey.Infrastructure.Persistence;

namespace DynamicSurvey.Infrastructure.Services;

public class SurveyRuleService : ISurveyRuleService
{
    private readonly AppDbContext _context;

    public SurveyRuleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateRuleAsync(CreateSurveyRuleDto dto, CancellationToken cancellationToken)
    {
        var rule = new SurveyRule
        {
            SurveyId = dto.SurveyId,
            Name = dto.Name,
            Description = dto.Description,
            Priority = dto.Priority,
            StopProcessing = dto.StopProcessing,
            IsActive = true,
            Conditions = dto.Conditions.Select(x => new SurveyRuleCondition
            {
                QuestionId = x.QuestionId,
                Operator = x.Operator,
                ExpectedValue = x.ExpectedValue,
                LogicalGroup = x.LogicalGroup,
                IsActive = true
            }).ToList(),
            Actions = dto.Actions.Select(x => new SurveyRuleAction
            {
                ActionType = x.ActionType,
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