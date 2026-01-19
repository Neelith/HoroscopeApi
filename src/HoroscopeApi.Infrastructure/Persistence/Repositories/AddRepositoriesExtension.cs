using Microsoft.Extensions.DependencyInjection;

namespace HoroscopeApi.Infrastructure.Persistence.Repositories;

internal static class AddRepositoriesExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services;
    }
}
