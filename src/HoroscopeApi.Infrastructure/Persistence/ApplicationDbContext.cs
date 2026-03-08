using System.Reflection;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Application.Infrastructure.User;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Domain.ApiKeys;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.Shared;
using HoroscopeApi.Domain.ZodiacSigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HoroscopeApi.Infrastructure.Persistence;

internal class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDateTimeProvider dateTimeProvider,
    ICurrentUserService currentUserService)
    : DbContext(options), IUnitOfWork
{
    public DbSet<ZodiacSignInfo> ZodiacSigns { get; set; }
    public DbSet<Horoscope> Horoscopes { get; set; }
    public DbSet<HoroscopePromptTemplate> HoroscopePromptTemplates { get; set; }
    public DbSet<Domain.ApiKeys.ApiKey> ApiKeys { get; set; }
    public DbSet<ApiKeyScope> ApiKeyScopes { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SetAuditablePropertiesOnCreatedEntities();

        SetAuditablePropertiesOnUpdatedEntities();

        return base.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (Database.CurrentTransaction is not null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        if (Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException("No transaction is in progress to commit.");
        }

        await Database.CurrentTransaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        if (Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException("No transaction is in progress to roll back.");
        }

        await Database.CurrentTransaction.RollbackAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void SetAuditablePropertiesOnCreatedEntities()
    {
        IEnumerable<EntityEntry<AuditableEntity>> entitiesBeignCreated = ChangeTracker.Entries<AuditableEntity>()
            .Where(entry => entry.State == EntityState.Added);

        string createdBy = currentUserService.IsCurrentUserAuthenticated()
            ? currentUserService.GetCurrentUserId()
            : "system";

        foreach (EntityEntry<AuditableEntity> entry in entitiesBeignCreated)
        {
            entry.Entity.CreatedAtUtc = dateTimeProvider.UtcNow;
            entry.Entity.CreatedBy = createdBy;
        }
    }

    private void SetAuditablePropertiesOnUpdatedEntities()
    {
        IEnumerable<EntityEntry<AuditableEntity>> entitiesBeignUpdated = ChangeTracker.Entries<AuditableEntity>()
            .Where(entry => entry.State == EntityState.Modified);

        string updatedBy = currentUserService.IsCurrentUserAuthenticated()
            ? currentUserService.GetCurrentUserId()
            : "system";

        foreach (EntityEntry<AuditableEntity> entry in entitiesBeignUpdated)
        {
            entry.Entity.UpdatedAtUtc = dateTimeProvider.UtcNow;
            entry.Entity.UpdatedBy = updatedBy;
        }
    }
}