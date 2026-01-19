using Microsoft.Extensions.DependencyInjection;
using HoroscopeApi.Application.Infrastructure.User;

namespace HoroscopeApi.Infrastructure.User;

internal static class AddCurrentUserServiceExtensions
{
    public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
