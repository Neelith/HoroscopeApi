using HoroscopeApi.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoroscopeApi.Infrastructure.Persistence.Configurations;

internal static class AuditableEntityExtensions
{
    public static void ConfigureAuditableEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : AuditableEntity
    {
        builder.Property(e => e.CreatedBy)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.UpdatedBy)
            .IsRequired(false);

        builder.Property(e => e.UpdatedAtUtc)
            .IsRequired(false);

        builder.Property(e => e.Deleted)
            .HasDefaultValue(false);

        // Add global query filter for soft delete
        builder.HasQueryFilter(e => !e.Deleted);
    }
}