using DynamicSurvey.Application.DTOs;

namespace DynamicSurvey.Application.Interfaces;

public interface ISurveyRuleService
{
    Task<int> CreateRuleAsync(CreateSurveyRuleDto dto, CancellationToken cancellationToken);
}