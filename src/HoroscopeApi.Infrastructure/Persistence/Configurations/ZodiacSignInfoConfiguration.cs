using HoroscopeApi.Domain.ZodiacSigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoroscopeApi.Infrastructure.Persistence.Configurations;

internal sealed class ZodiacSignInfoConfiguration : IEntityTypeConfiguration<ZodiacSignInfo>
{
    public void Configure(EntityTypeBuilder<ZodiacSignInfo> builder)
    {
        builder.ToTable("ZodiacSigns");

        builder.HasKey(z => z.Id);

        builder.Property(z => z.Sign)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(z => z.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(z => z.Symbol)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(z => z.StartMonth)
            .IsRequired();

        builder.Property(z => z.StartDay)
            .IsRequired();

        builder.Property(z => z.EndMonth)
            .IsRequired();

        builder.Property(z => z.EndDay)
            .IsRequired();

        builder.Property(z => z.Element)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(z => z.Quality)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(z => z.Polarity)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(z => z.RulingPlanet)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(z => z.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(z => z.Sign).IsUnique();

        builder.HasMany(z => z.Horoscopes)
            .WithOne(h => h.ZodiacSignInfo)
            .HasForeignKey(h => h.ZodiacSignId);
    }
}
