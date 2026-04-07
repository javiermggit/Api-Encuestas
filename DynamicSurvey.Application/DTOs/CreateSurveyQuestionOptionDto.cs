using System.ComponentModel.DataAnnotations;

namespace DynamicSurvey.Application.DTOs;

public class CreateSurveyQuestionOptionDto
{
    public int QuestionId { get; set; }

    [Required(ErrorMessage = "Value es obligatorio.")]
    [MaxLength(200, ErrorMessage = "Value no puede superar 200 caracteres.")]
    public string Value { get; set; } = string.Empty;

    [Required(ErrorMessage = "Label es obligatorio.")]
    [MaxLength(300, ErrorMessage = "Label no puede superar 300 caracteres.")]
    public string Label { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "DisplayOrder debe ser mayor que cero.")]
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
