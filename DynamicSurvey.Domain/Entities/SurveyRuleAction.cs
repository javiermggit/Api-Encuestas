using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Domain.Entities;

public class SurveyRuleAction
{
    public int Id { get; set; }
    public int RuleId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public int? TargetQuestionId { get; set; }
    public int? TargetSectionId { get; set; }
    public int OrderIndex { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public SurveyRule Rule { get; set; } = null!;
    public SurveyQuestion? TargetQuestion { get; set; }
    public SurveySection? TargetSection { get; set; }
}
