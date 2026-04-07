using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Domain.Entities
{
    public class Survey
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int Version { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<SurveySection> Sections { get; set; } = new List<SurveySection>();
        public ICollection<SurveyRule> Rules { get; set; } = new List<SurveyRule>();
        public ICollection<SurveySession> Sessions { get; set; } = new List<SurveySession>();
    }
}
