using DynamicSurvey.Application.DTOs;

namespace DynamicSurvey.Application.Interfaces;

public interface ISurveyDefinitionService
{
    Task<List<SurveyListItemDto>> GetAllActiveAsync(CancellationToken cancellationToken);
    Task<SurveyDefinitionDto?> GetByIdAsync(int surveyId, CancellationToken cancellationToken);
}