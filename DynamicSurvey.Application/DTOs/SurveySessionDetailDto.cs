using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Application.DTOs;

public class SurveySessionDetailDto
{
    public long Id { get; set; }
    public int SurveyId { get; set; }
    public string? SurveyName { get; set; }
    public string? UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? CurrentSectionId { get; set; }
    public string? CurrentSectionTitle { get; set; }
    public int? CurrentQuestionId { get; set; }
    public string? CurrentQuestionText { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<SurveySessionAnswerDto> Answers { get; set; } = new();
}
