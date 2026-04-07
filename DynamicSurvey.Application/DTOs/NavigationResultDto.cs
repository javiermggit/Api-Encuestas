using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Application.DTOs;

public class NavigationResultDto
{
    public bool Matched { get; set; }
    public string? ActionType { get; set; }
    public int? NextQuestionId { get; set; }
    public int? NextSectionId { get; set; }
    public bool EndSurvey { get; set; }
    public List<int> QuestionsToShow { get; set; } = new();
    public List<int> QuestionsToHide { get; set; } = new();
    public List<int> QuestionsToRequire { get; set; } = new();
}
