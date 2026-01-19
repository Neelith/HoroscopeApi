using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HoroscopeApi.Application.Infrastructure.Persistance;

namespace HoroscopeApi.Infrastructure.Persistence;

internal static class AddDbContextExtension
{
    public static IServiceCollection AddDbContext(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        services.AddDbContext<ApplicationDbContext>((options) => options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
