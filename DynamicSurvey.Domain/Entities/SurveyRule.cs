using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Domain.Entities;

public class SurveyRule
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; } = 1;
    public bool StopProcessing { get; set; } = true;
    public bool IsActive { get; set; } = true;

    public Survey Survey { get; set; } = null!;
    public ICollection<SurveyRuleCondition> Conditions { get; set; } = new List<SurveyRuleCondition>();
    public ICollection<SurveyRuleAction> Actions { get; set; } = new List<SurveyRuleAction>();
}
