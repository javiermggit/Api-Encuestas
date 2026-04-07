using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicSurvey.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/surveys/{surveyId:int}/rules")]
public class SurveyRulesController : ControllerBase
{
    private readonly ISurveyRuleService _ruleService;
    private readonly ISurveyAdminService _adminService;

    public SurveyRulesController(
        ISurveyRuleService ruleService,
        ISurveyAdminService adminService)
    {
        _ruleService = ruleService;
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBySurvey(int surveyId, CancellationToken cancellationToken)
    {
        var rules = await _adminService.GetRulesBySurveyAsync(surveyId, cancellationToken);
        return Ok(rules);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int surveyId, [FromBody] CreateSurveyRuleDto dto, CancellationToken cancellationToken)
    {
        dto.SurveyId = surveyId;
        var id = await _ruleService.CreateRuleAsync(dto, cancellationToken);
        return Ok(new { id });
    }
}