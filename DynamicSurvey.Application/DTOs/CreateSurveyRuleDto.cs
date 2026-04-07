using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Application.DTOs;

public class CreateSurveyRuleDto
{
    public int SurveyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public bool StopProcessing { get; set; }
    public List<CreateSurveyRuleConditionDto> Conditions { get; set; } = new();
    public List<CreateSurveyRuleActionDto> Actions { get; set; } = new();
}
