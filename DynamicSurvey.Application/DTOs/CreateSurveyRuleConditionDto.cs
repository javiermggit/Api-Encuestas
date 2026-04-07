using System.ComponentModel.DataAnnotations;

namespace DynamicSurvey.Application.DTOs;

public class CreateSurveyRuleConditionDto
{
    [Range(1, int.MaxValue, ErrorMessage = "QuestionId debe ser mayor que cero.")]
    public int QuestionId { get; set; }

    [Required(ErrorMessage = "Operator es obligatorio.")]
    [MaxLength(50, ErrorMessage = "Operator no puede superar 50 caracteres.")]
    public string Operator { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "ExpectedValue no puede superar 500 caracteres.")]
    public string? ExpectedValue { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "LogicalGroup debe ser mayor que cero.")]
    public int LogicalGroup { get; set; } = 1;
}
