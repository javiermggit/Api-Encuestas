using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Domain.Entities;

public class SurveyRuleCondition
{
    public int Id { get; set; }
    public int RuleId { get; set; }
    public int QuestionId { get; set; }
    public string Operator { get; set; } = string.Empty;
    public string? ExpectedValue { get; set; }
    public int LogicalGroup { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public SurveyRule Rule { get; set; } = null!;
    public SurveyQuestion Question { get; set; } = null!;
}
