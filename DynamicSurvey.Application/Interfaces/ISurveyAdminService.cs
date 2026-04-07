using DynamicSurvey.Application.DTOs;


namespace DynamicSurvey.Application.Interfaces;

public interface ISurveyAdminService
{
    Task<int> CreateSurveyAsync(CreateSurveyDto dto, CancellationToken cancellationToken);
    Task<int> CreateSectionAsync(CreateSurveySectionDto dto, CancellationToken cancellationToken);
    Task<int> CreateQuestionAsync(CreateSurveyQuestionDto dto, CancellationToken cancellationToken);
    Task<int> CreateOptionAsync(CreateSurveyQuestionOptionDto dto, CancellationToken cancellationToken);
    Task<List<SurveyRuleListItemDto>> GetRulesBySurveyAsync(int surveyId, CancellationToken cancellationToken);

    Task<List<SurveyListItemDto>> GetSurveysAsync(CancellationToken cancellationToken);
    Task<SurveyAdminDetailDto?> GetSurveyByIdAsync(int surveyId, CancellationToken cancellationToken);
    Task<List<SurveyQuestionDto>> GetQuestionsBySectionAsync(int sectionId, CancellationToken cancellationToken);
    Task<List<SurveyQuestionOptionDto>> GetOptionsByQuestionAsync(int questionId, CancellationToken cancellationToken);

    Task<bool> UpdateSurveyAsync(int surveyId, UpdateSurveyDto dto, CancellationToken cancellationToken);
}