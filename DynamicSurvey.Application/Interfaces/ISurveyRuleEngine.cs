using DynamicSurvey.Application.DTOs;

namespace DynamicSurvey.Application.Interfaces;

public interface ISurveyRuleEngine
{
    Task<NavigationResultDto> EvaluateAsync(
        int surveyId,
        int currentQuestionId,
        string? answerValue,
        CancellationToken cancellationToken);
}