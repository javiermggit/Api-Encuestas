using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using DynamicSurvey.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicSurvey.Infrastructure.Services;

public class SurveyRuleEngine : ISurveyRuleEngine
{
    private readonly AppDbContext _context;

    public SurveyRuleEngine(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NavigationResultDto> EvaluateAsync(
        int surveyId,
        int currentQuestionId,
        string? answerValue,
        CancellationToken cancellationToken)
    {
        var normalizedAnswer = answerValue?.Trim().ToUpper();

        var rules = await _context.SurveyRules
            .AsNoTracking()
            .Where(r => r.SurveyId == surveyId && r.IsActive)
            .OrderBy(r => r.Priority)
            .Include(r => r.Conditions)
            .Include(r => r.Actions)
            .ToListAsync(cancellationToken);

        foreach (var rule in rules)
        {
            var conditions = rule.Conditions
                .Where(c => c.IsActive)
                .ToList();

            if (!conditions.Any())
                continue;

            var allConditionsMatch = conditions.All(c =>
                EvaluateCondition(
                    c.QuestionId,
                    currentQuestionId,
                    c.Operator,
                    c.ExpectedValue,
                    normalizedAnswer));

            if (!allConditionsMatch)
                continue;

            var result = new NavigationResultDto
            {
                Matched = true
            };

            var actions = rule.Actions
                .Where(a => a.IsActive)
                .OrderBy(a => a.OrderIndex)
                .ToList();

            foreach (var action in actions)
            {
                switch (action.ActionType)
                {
                    case "GoToQuestion":
                        result.ActionType = action.ActionType;
                        result.NextQuestionId = action.TargetQuestionId;
                        break;

                    case "GoToSection":
                        result.ActionType = action.ActionType;
                        result.NextSectionId = action.TargetSectionId;
                        break;

                    case "ShowQuestion":
                        if (action.TargetQuestionId.HasValue)
                            result.QuestionsToShow.Add(action.TargetQuestionId.Value);
                        break;

                    case "HideQuestion":
                        if (action.TargetQuestionId.HasValue)
                            result.QuestionsToHide.Add(action.TargetQuestionId.Value);
                        break;

                    case "SetRequired":
                        if (action.TargetQuestionId.HasValue)
                            result.QuestionsToRequire.Add(action.TargetQuestionId.Value);
                        break;

                    case "EndSurvey":
                        result.EndSurvey = true;
                        break;
                }
            }

            if (rule.StopProcessing)
                return result;
        }

        return new NavigationResultDto
        {
            Matched = false
        };
    }

    private static bool EvaluateCondition(
        int conditionQuestionId,
        int currentQuestionId,
        string op,
        string? expectedValue,
        string? currentAnswerValue)
    {
        if (conditionQuestionId != currentQuestionId)
            return false;

        var normalizedExpected = expectedValue?.Trim().ToUpper();
        var normalizedCurrent = currentAnswerValue?.Trim().ToUpper();

        return op switch
        {
            "Equals" => normalizedCurrent == normalizedExpected,
            "NotEquals" => normalizedCurrent != normalizedExpected,
            "Contains" => normalizedCurrent != null && normalizedExpected != null && normalizedCurrent.Contains(normalizedExpected),
            "IsEmpty" => string.IsNullOrWhiteSpace(normalizedCurrent),
            "IsNotEmpty" => !string.IsNullOrWhiteSpace(normalizedCurrent),
            _ => false
        };
    }
}