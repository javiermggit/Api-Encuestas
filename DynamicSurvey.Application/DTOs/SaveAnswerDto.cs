using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Application.DTOs;

public class SaveAnswerDto
{
    public int QuestionId { get; set; }
    public string? AnswerValue { get; set; }
    public List<int>? OptionIds { get; set; }
}
