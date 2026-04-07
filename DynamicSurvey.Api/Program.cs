using DynamicSurvey.Api.Middleware;
using DynamicSurvey.Infrastructure.Persistence;
using DynamicSurvey.Infrastructure.Services;
using DynamicSurvey.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Valor inválido." : e.ErrorMessage)
                    .ToArray());

        return new BadRequestObjectResult(new
        {
            message = "La solicitud contiene errores de validación.",
            errors
        });
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClients", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://dynamic-survey-web.vercel.app",
                "https://dynamic-survey-web.netlify.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISurveyRuleService, SurveyRuleService>();
builder.Services.AddScoped<ISurveyRuleEngine, SurveyRuleEngine>();
builder.Services.AddScoped<ISurveySessionService, SurveySessionService>();
builder.Services.AddScoped<ISurveyDefinitionService, SurveyDefinitionService>();
builder.Services.AddScoped<ISurveyAdminService, SurveyAdminService>();

var app = builder.Build();

app.UseGlobalExceptionHandling();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AngularClients");
app.UseAuthorization();
app.MapControllers();

app.Run();