using System.ComponentModel.DataAnnotations;

namespace DynamicSurvey.Application.DTOs;

public class CreateSurveySectionDto
{
    public int SurveyId { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El título no puede superar 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "La descripción no puede superar 1000 caracteres.")]
    public string? Description { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "DisplayOrder debe ser mayor que cero.")]
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
