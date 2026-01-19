using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Infrastructure.Persistence;

public static class AddDatabaseMigrationsExtension
{
    public static void ApplyDatabaseMigrations(IServiceScope scope, ILogger logger)
    {
        using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            if (dbContext.Database.CanConnect())
            {
                logger.LogInformation("Database connection successful. Applying migrations...");
                dbContext.Database.Migrate();
            }
            else
            {
                logger.LogWarning("Cannot connect to the database. Migrations will not be applied.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }
    }
}
