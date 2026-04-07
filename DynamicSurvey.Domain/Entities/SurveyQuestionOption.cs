using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DynamicSurvey.Domain.Entities;

public class SurveyQuestionOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public SurveyQuestion Question { get; set; } = null!;
}
