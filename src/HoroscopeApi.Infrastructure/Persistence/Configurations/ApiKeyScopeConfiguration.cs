using HoroscopeApi.Domain.ApiKeys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoroscopeApi.Infrastructure.Persistence.Configurations;

public class ApiKeyScopeConfiguration : IEntityTypeConfiguration<ApiKeyScope>
{
    public void Configure(EntityTypeBuilder<ApiKeyScope> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.ToTable("ApiKeyScopes");

        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.ApiKey)
            .WithMany(a => a.Scopes);

        builder.Property(a => a.ApiKeyId)
            .IsRequired();

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(a => a.Name)
            .IsUnique(false);
    }
}