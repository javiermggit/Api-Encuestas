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
        long sessionId,
        int surveyId,
        int currentQuestionId,
        string? answerValue,
        CancellationToken cancellationToken)
    {
        var normalizedAnswer = Normalize(answerValue);

        var answers = await _context.SurveyAnswers
            .AsNoTracking()
            .Where(a => a.SessionId == sessionId)
            .ToDictionaryAsync(a => a.QuestionId, a => Normalize(a.AnswerValue), cancellationToken);

        answers[currentQuestionId] = normalizedAnswer;

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

            if (conditions.Count == 0)
                continue;

            var ruleMatched = conditions
                .GroupBy(c => c.LogicalGroup)
                .Any(group => group.All(condition =>
                    EvaluateCondition(condition.Operator, condition.ExpectedValue, answers.GetValueOrDefault(condition.QuestionId))));

            if (!ruleMatched)
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

        return new NavigationResultDto { Matched = false };
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

    private static bool EvaluateCondition(string op, string? expectedValue, string? currentAnswerValue)
    {
        var normalizedExpected = Normalize(expectedValue);

        return op switch
        {
            "Equals" => currentAnswerValue == normalizedExpected,
            "NotEquals" => currentAnswerValue != normalizedExpected,
            "Contains" => currentAnswerValue != null && normalizedExpected != null && currentAnswerValue.Contains(normalizedExpected),
            "IsEmpty" => string.IsNullOrWhiteSpace(currentAnswerValue),
            "IsNotEmpty" => !string.IsNullOrWhiteSpace(currentAnswerValue),
            _ => false
        };
    }
}
