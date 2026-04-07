using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Domain.Entities;

public class SurveyQuestion
{
    public int Id { get; set; }
    public int SectionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public SurveySection Section { get; set; } = null!;
    public ICollection<SurveyQuestionOption> Options { get; set; } = new List<SurveyQuestionOption>();
}
