using DynamicSurvey.Application.DTOs;

namespace DynamicSurvey.Application.Interfaces;

public interface ISurveySessionService
{
    Task<long> CreateSessionAsync(CreateSessionDto dto, CancellationToken cancellationToken);

    Task<SurveySessionDetailDto?> GetByIdAsync(long sessionId, CancellationToken cancellationToken);

    Task<NavigationResultDto> SaveAnswerAndNavigateAsync(
        long sessionId,
        SaveAnswerDto dto,
        CancellationToken cancellationToken);
}