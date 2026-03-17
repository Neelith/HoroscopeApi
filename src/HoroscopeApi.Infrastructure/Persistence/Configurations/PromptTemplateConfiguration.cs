using HoroscopeApi.Domain.Prompts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoroscopeApi.Infrastructure.Persistence.Configurations;

internal sealed class PromptTemplateConfiguration : IEntityTypeConfiguration<PromptTemplate>
{
    public void Configure(EntityTypeBuilder<PromptTemplate> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.ToTable("PromptTemplates");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.SystemPrompt)
            .IsRequired();

        builder.Property(t => t.FewShotExamples)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(t => t.UserPromptTemplate)
            .IsRequired();

        builder.HasIndex(t => t.Type).IsUnique();

        builder.HasQueryFilter(t => !t.Deleted);
    }
}