using DynamicSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DynamicSurvey.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Survey> Surveys => Set<Survey>();
    public DbSet<SurveySection> SurveySections => Set<SurveySection>();
    public DbSet<SurveyQuestion> SurveyQuestions => Set<SurveyQuestion>();
    public DbSet<SurveyQuestionOption> SurveyQuestionOptions => Set<SurveyQuestionOption>();
    public DbSet<SurveyRule> SurveyRules => Set<SurveyRule>();
    public DbSet<SurveyRuleCondition> SurveyRuleConditions => Set<SurveyRuleCondition>();
    public DbSet<SurveyRuleAction> SurveyRuleActions => Set<SurveyRuleAction>();
    public DbSet<SurveySession> SurveySessions => Set<SurveySession>();
    public DbSet<SurveyAnswer> SurveyAnswers => Set<SurveyAnswer>();
    public DbSet<SurveyAnswerOption> SurveyAnswerOptions => Set<SurveyAnswerOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Survey>().ToTable("Survey");
        modelBuilder.Entity<SurveySection>().ToTable("SurveySection");
        modelBuilder.Entity<SurveyQuestion>().ToTable("SurveyQuestion");
        modelBuilder.Entity<SurveyQuestionOption>().ToTable("SurveyQuestionOption");
        modelBuilder.Entity<SurveyRule>().ToTable("SurveyRule");
        modelBuilder.Entity<SurveyRuleCondition>().ToTable("SurveyRuleCondition");
        modelBuilder.Entity<SurveyRuleAction>().ToTable("SurveyRuleAction");
        modelBuilder.Entity<SurveySession>().ToTable("SurveySession");
        modelBuilder.Entity<SurveyAnswer>().ToTable("SurveyAnswer");
        modelBuilder.Entity<SurveyAnswerOption>().ToTable("SurveyAnswerOption");

        modelBuilder.Entity<SurveySection>()
            .HasOne(x => x.Survey)
            .WithMany(x => x.Sections)
            .HasForeignKey(x => x.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyQuestion>()
            .HasOne(x => x.Section)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyQuestionOption>()
            .HasOne(x => x.Question)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyRule>()
            .HasOne(x => x.Survey)
            .WithMany(x => x.Rules)
            .HasForeignKey(x => x.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyRuleCondition>()
            .HasOne(x => x.Rule)
            .WithMany(x => x.Conditions)
            .HasForeignKey(x => x.RuleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyRuleCondition>()
            .HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SurveyRuleAction>()
            .HasOne(x => x.Rule)
            .WithMany(x => x.Actions)
            .HasForeignKey(x => x.RuleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyRuleAction>()
            .HasOne(x => x.TargetQuestion)
            .WithMany()
            .HasForeignKey(x => x.TargetQuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SurveyRuleAction>()
            .HasOne(x => x.TargetSection)
            .WithMany()
            .HasForeignKey(x => x.TargetSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SurveySession>()
            .HasOne(x => x.Survey)
            .WithMany(x => x.Sessions)
            .HasForeignKey(x => x.SurveyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SurveyAnswer>()
            .HasOne(x => x.Session)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyAnswer>()
            .HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SurveyAnswerOption>()
            .HasOne(x => x.Answer)
            .WithMany(x => x.AnswerOptions)
            .HasForeignKey(x => x.AnswerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyAnswerOption>()
            .HasOne(x => x.Option)
            .WithMany()
            .HasForeignKey(x => x.OptionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SurveyAnswer>()
            .HasIndex(x => new { x.SessionId, x.QuestionId })
            .IsUnique();

        modelBuilder.Entity<SurveyAnswerOption>()
            .HasIndex(x => new { x.AnswerId, x.OptionId })
            .IsUnique();
    }
}