using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Application.DTOs;

public class CreateSurveyRuleActionDto
{
    public string ActionType { get; set; } = string.Empty;
    public int? TargetQuestionId { get; set; }
    public int? TargetSectionId { get; set; }
    public int OrderIndex { get; set; } = 1;
}
