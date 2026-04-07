using DynamicSurvey.Application.DTOs;

namespace DynamicSurvey.Application.Interfaces;

public interface ISurveyRuleEngine
{
    Task<NavigationResultDto> EvaluateAsync(
        long sessionId,
        int surveyId,
        int currentQuestionId,
        string? answerValue,
        CancellationToken cancellationToken);
}
