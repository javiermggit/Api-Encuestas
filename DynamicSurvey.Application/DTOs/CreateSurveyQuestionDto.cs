using System.ComponentModel.DataAnnotations;

namespace DynamicSurvey.Application.DTOs;

public class CreateSurveyQuestionDto
{
    public int SectionId { get; set; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El código no puede superar 50 caracteres.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "El texto es obligatorio.")]
    [MaxLength(1000, ErrorMessage = "El texto no puede superar 1000 caracteres.")]
    public string Text { get; set; } = string.Empty;

    [Required(ErrorMessage = "QuestionType es obligatorio.")]
    [MaxLength(50, ErrorMessage = "QuestionType no puede superar 50 caracteres.")]
    public string QuestionType { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "DisplayOrder debe ser mayor que cero.")]
    public int DisplayOrder { get; set; }

    public bool IsRequired { get; set; }

    [MaxLength(500, ErrorMessage = "Placeholder no puede superar 500 caracteres.")]
    public string? Placeholder { get; set; }

    [MaxLength(1000, ErrorMessage = "HelpText no puede superar 1000 caracteres.")]
    public string? HelpText { get; set; }

    public bool IsActive { get; set; } = true;
}
