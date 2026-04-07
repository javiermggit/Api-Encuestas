namespace DynamicSurvey.Application.DTOs;

public class SessionProgressDto
{
    public long SessionId { get; set; }
    public int SurveyId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public decimal CompletionPercentage { get; set; }
    public int? CurrentSectionId { get; set; }
    public string? CurrentSectionTitle { get; set; }
    public int? CurrentQuestionId { get; set; }
    public string? CurrentQuestionText { get; set; }
}
