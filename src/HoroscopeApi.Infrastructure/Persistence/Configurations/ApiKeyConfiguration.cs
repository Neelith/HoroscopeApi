using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoroscopeApi.Infrastructure.Persistence.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<Domain.ApiKeys.ApiKey>
{
    public void Configure(EntityTypeBuilder<Domain.ApiKeys.ApiKey> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.ToTable("ApiKeys");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Prefix)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(a => a.Algorithm)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(a => a.Hash)
            .IsRequired();

        builder.Property(a => a.Salt)
            .IsRequired();

        builder.Property(a => a.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.RateLimitType)
            .IsRequired()
            .HasConversion<string>();

        builder.HasIndex(a => a.Algorithm)
            .IsUnique(false);

        builder.HasIndex(a => a.Hash)
            .IsUnique();

        builder.HasIndex(a => a.Type)
            .IsUnique(false);

        builder.HasIndex(a => a.RateLimitType)
            .IsUnique(false);

        builder.HasIndex(a => a.Prefix)
            .IsUnique(false);
    }
}