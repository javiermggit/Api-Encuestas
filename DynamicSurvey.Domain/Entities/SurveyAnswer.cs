using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Domain.Entities;

public class SurveyAnswer
{
    public long Id { get; set; }
    public long SessionId { get; set; }
    public int QuestionId { get; set; }
    public string? AnswerValue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public SurveySession Session { get; set; } = null!;
    public SurveyQuestion Question { get; set; } = null!;
    public ICollection<SurveyAnswerOption> AnswerOptions { get; set; } = new List<SurveyAnswerOption>();
}
