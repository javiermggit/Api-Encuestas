using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicSurvey.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/questions/{questionId:int}/options")]
public class AdminOptionsController : ControllerBase
{
    private readonly ISurveyAdminService _adminService;

    public AdminOptionsController(ISurveyAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int questionId, [FromBody] CreateSurveyQuestionOptionDto dto, CancellationToken cancellationToken)
    {
        dto.QuestionId = questionId;
        var id = await _adminService.CreateOptionAsync(dto, cancellationToken);
        return Ok(new { id });
    }
}