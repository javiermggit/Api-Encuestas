using System.ComponentModel.DataAnnotations;

namespace DynamicSurvey.Application.DTOs;

public class CreateSurveyDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El nombre no puede superar 200 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "La descripción no puede superar 1000 caracteres.")]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "La versión debe ser mayor que cero.")]
    public int Version { get; set; } = 1;
}
