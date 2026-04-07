using System.ComponentModel.DataAnnotations;

namespace DynamicSurvey.Application.DTOs;

public class SaveAnswerDto
{
    [Range(1, int.MaxValue, ErrorMessage = "QuestionId debe ser mayor que cero.")]
    public int QuestionId { get; set; }

    [MaxLength(2000, ErrorMessage = "AnswerValue no puede superar 2000 caracteres.")]
    public string? AnswerValue { get; set; }

    public List<int>? OptionIds { get; set; }
}
