using DynamicSurvey.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicSurvey.Api.Controllers.Public;

[ApiController]
[Route("api/surveys")]
public class SurveysController : ControllerBase
{
    private readonly ISurveyDefinitionService _service;

    public SurveysController(ISurveyDefinitionService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var survey = await _service.GetByIdAsync(id, cancellationToken);

        if (survey is null)
            return NotFound(new { message = "Encuesta no encontrada." });

        return Ok(survey);
    }
}