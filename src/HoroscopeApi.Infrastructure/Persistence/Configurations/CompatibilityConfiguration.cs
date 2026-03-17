using HoroscopeApi.Domain.Compatibilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoroscopeApi.Infrastructure.Persistence.Configurations;

internal sealed class CompatibilityConfiguration : IEntityTypeConfiguration<Compatibility>
{
    public void Configure(EntityTypeBuilder<Compatibility> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.ToTable("Compatibilities");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstZodiacSignId)
            .IsRequired();

        builder.HasOne(c => c.FirstZodiacSignInfo)
            .WithMany()
            .HasForeignKey(c => c.FirstZodiacSignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.SecondZodiacSignId)
            .IsRequired();

        builder.HasOne(c => c.SecondZodiacSignInfo)
            .WithMany()
            .HasForeignKey(c => c.SecondZodiacSignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.Score)
            .IsRequired();

        builder.Property(c => c.Description)
            .IsRequired();

        builder.Property(c => c.Deleted)
            .HasDefaultValue(false);

        // Unique index on sign pair — lower ID always first
        builder.HasIndex(c => new { c.FirstZodiacSignId, c.SecondZodiacSignId })
            .IsUnique();

        // Query filter for soft delete
        builder.HasQueryFilter(c => !c.Deleted);
    }
}