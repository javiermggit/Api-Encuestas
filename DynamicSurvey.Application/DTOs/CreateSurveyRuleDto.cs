using System.ComponentModel.DataAnnotations;

namespace DynamicSurvey.Application.DTOs;

public class CreateSurveyRuleDto
{
    public int SurveyId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El nombre no puede superar 200 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "La descripción no puede superar 1000 caracteres.")]
    public string? Description { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Priority debe ser mayor que cero.")]
    public int Priority { get; set; }

    public bool StopProcessing { get; set; }
    public List<CreateSurveyRuleConditionDto> Conditions { get; set; } = new();
    public List<CreateSurveyRuleActionDto> Actions { get; set; } = new();
}
