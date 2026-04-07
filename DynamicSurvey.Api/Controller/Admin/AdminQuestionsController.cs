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

    [HttpGet]
    public async Task<IActionResult> GetBySection(int sectionId, CancellationToken cancellationToken)
    {
        var questions = await _adminService.GetQuestionsBySectionAsync(sectionId, cancellationToken);
        return Ok(questions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int sectionId, [FromBody] CreateSurveyQuestionDto dto, CancellationToken cancellationToken)
    {
        dto.SectionId = sectionId;
        var id = await _adminService.CreateQuestionAsync(dto, cancellationToken);
        return Ok(new { id });
    }
}