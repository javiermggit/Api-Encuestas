using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Domain.Entities;

public class SurveySection
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Survey Survey { get; set; } = null!;
    public ICollection<SurveyQuestion> Questions { get; set; } = new List<SurveyQuestion>();
}