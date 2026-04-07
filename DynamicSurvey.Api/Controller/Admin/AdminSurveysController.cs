using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicSurvey.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/surveys")]
public class AdminSurveysController : ControllerBase
{
    private readonly ISurveyAdminService _adminService;

    public AdminSurveysController(ISurveyAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSurveyDto dto, CancellationToken cancellationToken)
    {
        var id = await _adminService.CreateSurveyAsync(dto, cancellationToken);
        return Ok(new { id });
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var surveys = await _adminService.GetSurveysAsync(cancellationToken);
        return Ok(surveys);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSurveyDto dto, CancellationToken cancellationToken)
    {
        var updated = await _adminService.UpdateSurveyAsync(id, dto, cancellationToken);

        if (!updated)
            return NotFound(new { message = "Encuesta no encontrada." });

        return Ok(new { message = "Encuesta actualizada correctamente." });
    }
}