using System.Reflection;
using Microsoft.EntityFrameworkCore;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Application.Infrastructure.User;
using HoroscopeApi.Shared.Domain;
using HoroscopeApi.Shared.Time;

namespace HoroscopeApi.Infrastructure.Persistence
{
    internal class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
        : DbContext(options), IUnitOfWork
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SetAuditablePropertiesOnCreatedEntities();

            SetAuditablePropertiesOnUpdatedEntities();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditablePropertiesOnCreatedEntities()
        {
            var entitiesBeignCreated = ChangeTracker.Entries<AuditableEntity>()
                .Where(entry => entry.State == EntityState.Added);

            string createdBy = currentUserService.IsCurrentUserAuthenticated()
                ? currentUserService.GetCurrentUserId()
                : "system";

            foreach (var entry in entitiesBeignCreated)
            {
                entry.Entity.CreatedAtUtc = dateTimeProvider.UtcNow;
                entry.Entity.CreatedBy = createdBy;
            }
        }

        private void SetAuditablePropertiesOnUpdatedEntities()
        {
            var entitiesBeignUpdated = ChangeTracker.Entries<AuditableEntity>()
                .Where(entry => entry.State == EntityState.Modified);

            string updatedBy = currentUserService.IsCurrentUserAuthenticated()
                ? currentUserService.GetCurrentUserId()
                : "system";

            foreach (var entry in entitiesBeignUpdated)
            {
                entry.Entity.UpdatedAtUtc = dateTimeProvider.UtcNow;
                entry.Entity.UpdatedBy = updatedBy;
            }
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
    }
}
