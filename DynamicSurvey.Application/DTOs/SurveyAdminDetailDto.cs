namespace DynamicSurvey.Application.DTOs;

public class SurveyAdminDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<SurveySectionDto> Sections { get; set; } = new();
}
