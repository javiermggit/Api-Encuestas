using System.ComponentModel.DataAnnotations;

namespace DynamicSurvey.Application.DTOs;

public class CreateSurveyRuleActionDto
{
    [Required(ErrorMessage = "ActionType es obligatorio.")]
    [MaxLength(50, ErrorMessage = "ActionType no puede superar 50 caracteres.")]
    public string ActionType { get; set; } = string.Empty;

    public int? TargetQuestionId { get; set; }
    public int? TargetSectionId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "OrderIndex debe ser mayor que cero.")]
    public int OrderIndex { get; set; } = 1;
}
