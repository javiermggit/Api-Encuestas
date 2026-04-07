using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicSurvey.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/surveys/{surveyId:int}/sections")]
public class AdminSectionsController : ControllerBase
{
    private readonly ISurveyAdminService _adminService;

    public AdminSectionsController(ISurveyAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int surveyId, [FromBody] CreateSurveySectionDto dto, CancellationToken cancellationToken)
    {
        dto.SurveyId = surveyId;
        var id = await _adminService.CreateSectionAsync(dto, cancellationToken);
        return Ok(new { id });
    }
}