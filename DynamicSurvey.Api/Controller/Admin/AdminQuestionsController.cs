using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicSurvey.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/sections/{sectionId:int}/questions")]
public class AdminQuestionsController : ControllerBase
{
    private readonly ISurveyAdminService _adminService;

    public AdminQuestionsController(ISurveyAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int sectionId, [FromBody] CreateSurveyQuestionDto dto, CancellationToken cancellationToken)
    {
        dto.SectionId = sectionId;
        var id = await _adminService.CreateQuestionAsync(dto, cancellationToken);
        return Ok(new { id });
    }
}