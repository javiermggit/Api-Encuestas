using System.ComponentModel.DataAnnotations;

namespace DynamicSurvey.Application.DTOs;

public class CreateSessionDto
{
    [Range(1, int.MaxValue, ErrorMessage = "SurveyId debe ser mayor que cero.")]
    public int SurveyId { get; set; }

    [MaxLength(100, ErrorMessage = "UserId no puede superar 100 caracteres.")]
    public string? UserId { get; set; }
}
