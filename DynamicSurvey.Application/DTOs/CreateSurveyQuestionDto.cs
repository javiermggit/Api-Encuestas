using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Application.DTOs;

public class CreateSurveyQuestionDto
{
    public int SectionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public bool IsActive { get; set; } = true;
}
