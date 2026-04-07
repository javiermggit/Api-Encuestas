using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Application.DTOs;

public class SurveySessionAnswerDto
{
    public long AnswerId { get; set; }
    public int QuestionId { get; set; }
    public string? QuestionCode { get; set; }
    public string? QuestionText { get; set; }
    public string? AnswerValue { get; set; }
    public List<int> OptionIds { get; set; } = new();
}
