using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicSurvey.Api.Controllers.Public;

[ApiController]
[Route("api/sessions")]
public class SessionsController : ControllerBase
{
    private readonly ISurveySessionService _sessionService;

    public SessionsController(ISurveySessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSessionDto dto, CancellationToken cancellationToken)
    {
        var sessionId = await _sessionService.CreateSessionAsync(dto, cancellationToken);
        return Ok(new { sessionId });
    }

    [HttpGet("{sessionId:long}")]
    public async Task<IActionResult> GetById(long sessionId, CancellationToken cancellationToken)
    {
        var session = await _sessionService.GetByIdAsync(sessionId, cancellationToken);

        if (session is null)
            return NotFound(new { message = "La sesión no existe." });

        return Ok(session);
    }

    [HttpGet("{sessionId:long}/progress")]
    public async Task<IActionResult> GetProgress(long sessionId, CancellationToken cancellationToken)
    {
        var progress = await _sessionService.GetProgressAsync(sessionId, cancellationToken);

        if (progress is null)
            return NotFound(new { message = "La sesión no existe." });

        return Ok(progress);
    }

    [HttpPost("{sessionId:long}/complete")]
    public async Task<IActionResult> Complete(long sessionId, CancellationToken cancellationToken)
    {
        var completed = await _sessionService.CompleteSessionAsync(sessionId, cancellationToken);

        if (!completed)
            return NotFound(new { message = "La sesión no existe." });

        return Ok(new { message = "La sesión fue finalizada correctamente." });
    }

    [HttpPost("{sessionId:long}/answers")]
    public async Task<IActionResult> SaveAnswer(long sessionId, [FromBody] SaveAnswerDto dto, CancellationToken cancellationToken)
    {
        var result = await _sessionService.SaveAnswerAndNavigateAsync(sessionId, dto, cancellationToken);
        return Ok(result);
    }
}