using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Application.DTOs;

public class SurveyRuleActionSummaryDto
{
    public int Id { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public int? TargetQuestionId { get; set; }
    public int? TargetSectionId { get; set; }
    public int OrderIndex { get; set; }
}
