using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSurvey.Application.DTOs;

public class CreateSessionDto
{
    public int SurveyId { get; set; }
    public string? UserId { get; set; }
}
