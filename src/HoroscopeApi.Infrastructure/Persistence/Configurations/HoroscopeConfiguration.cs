using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoroscopeApi.Infrastructure.Persistence.Configurations;

internal sealed class HoroscopeConfiguration : IEntityTypeConfiguration<Horoscope>
{
    public void Configure(EntityTypeBuilder<Horoscope> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.ToTable("Horoscopes");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Sign)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(h => h.Period)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(h => h.Date)
            .IsRequired();

        builder.Property(h => h.GeneralPrediction)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(h => h.LovePrediction)
            .HasMaxLength(1000);

        builder.Property(h => h.CareerPrediction)
            .HasMaxLength(1000);

        builder.Property(h => h.HealthPrediction)
            .HasMaxLength(1000);

        builder.Property(h => h.LuckyNumbers)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.LuckyColors)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(h => h.MoodScore)
            .IsRequired();

        builder.Property(h => h.Keywords)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(h => h.Deleted)
            .HasDefaultValue(false);

        // Indexes for performance
        builder.HasIndex(h => new { h.Sign, h.Period, h.Date });
        builder.HasIndex(h => new { h.Sign, h.Date });

        // Query filter for soft delete
        builder.HasQueryFilter(h => !h.Deleted);
    }
}
