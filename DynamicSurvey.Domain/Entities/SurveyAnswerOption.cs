using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Domain.Entities;

public class SurveyAnswerOption
{
    public long Id { get; set; }
    public long AnswerId { get; set; }
    public int OptionId { get; set; }

    public SurveyAnswer Answer { get; set; } = null!;
    public SurveyQuestionOption Option { get; set; } = null!;
}
