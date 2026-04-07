using DynamicSurvey.Application.DTOs;

namespace DynamicSurvey.Application.Interfaces;

public interface ISurveyDefinitionService
{
    Task<SurveyDefinitionDto?> GetByIdAsync(int surveyId, CancellationToken cancellationToken);
}