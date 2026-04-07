using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Domain.Entities;

public class SurveySession
{
    public long Id { get; set; }
    public int SurveyId { get; set; }
    public string? UserId { get; set; }
    public string Status { get; set; } = "InProgress";
    public int? CurrentSectionId { get; set; }
    public int? CurrentQuestionId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Survey Survey { get; set; } = null!;
    public ICollection<SurveyAnswer> Answers { get; set; } = new List<SurveyAnswer>();
}
