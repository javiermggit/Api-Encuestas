using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Application.DTOs;

public class SurveyRuleConditionSummaryDto
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string Operator { get; set; } = string.Empty;
    public string? ExpectedValue { get; set; }
    public int LogicalGroup { get; set; }
}
