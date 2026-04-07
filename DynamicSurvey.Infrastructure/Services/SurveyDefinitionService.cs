using DynamicSurvey.Application.DTOs;
using DynamicSurvey.Application.Interfaces;
using DynamicSurvey.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicSurvey.Infrastructure.Services;

public class SurveyDefinitionService : ISurveyDefinitionService
{
    private readonly AppDbContext _context;

    public SurveyDefinitionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SurveyDefinitionDto?> GetByIdAsync(int surveyId, CancellationToken cancellationToken)
    {
        var survey = await _context.Surveys
            .AsNoTracking()
            .Include(x => x.Sections.Where(s => s.IsActive).OrderBy(s => s.DisplayOrder))
                .ThenInclude(s => s.Questions.Where(q => q.IsActive).OrderBy(q => q.DisplayOrder))
                    .ThenInclude(q => q.Options.Where(o => o.IsActive).OrderBy(o => o.DisplayOrder))
            .FirstOrDefaultAsync(x => x.Id == surveyId && x.IsActive, cancellationToken);

        if (survey is null)
            return null;

        return new SurveyDefinitionDto
        {
            Id = survey.Id,
            Name = survey.Name,
            Description = survey.Description,
            Sections = survey.Sections
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new SurveySectionDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    DisplayOrder = s.DisplayOrder,
                    Questions = s.Questions
                        .OrderBy(q => q.DisplayOrder)
                        .Select(q => new SurveyQuestionDto
                        {
                            Id = q.Id,
                            Code = q.Code,
                            Text = q.Text,
                            QuestionType = q.QuestionType,
                            DisplayOrder = q.DisplayOrder,
                            IsRequired = q.IsRequired,
                            Placeholder = q.Placeholder,
                            HelpText = q.HelpText,
                            Options = q.Options
                                .OrderBy(o => o.DisplayOrder)
                                .Select(o => new SurveyQuestionOptionDto
                                {
                                    Id = o.Id,
                                    Value = o.Value,
                                    Label = o.Label,
                                    DisplayOrder = o.DisplayOrder
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}