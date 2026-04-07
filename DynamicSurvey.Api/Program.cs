using DynamicSurvey.Application.Interfaces;
using DynamicSurvey.Infrastructure.Persistence;
using DynamicSurvey.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AngularDev");

app.UseAuthorization();
app.MapControllers();

app.Run();
