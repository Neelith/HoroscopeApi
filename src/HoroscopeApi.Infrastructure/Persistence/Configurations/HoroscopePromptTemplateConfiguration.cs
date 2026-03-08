using HoroscopeApi.Domain.Horoscopes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoroscopeApi.Infrastructure.Persistence.Configurations;

internal sealed class HoroscopePromptTemplateConfiguration : IEntityTypeConfiguration<HoroscopePromptTemplate>
{
    public void Configure(EntityTypeBuilder<HoroscopePromptTemplate> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.ToTable("HoroscopePromptTemplates");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Period)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.SystemPrompt)
            .IsRequired();

        builder.Property(t => t.FewShotExamples)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(t => t.UserPromptTemplate)
            .IsRequired();

        builder.HasIndex(t => t.Period).IsUnique();

        builder.HasQueryFilter(t => !t.Deleted);
    }
}
