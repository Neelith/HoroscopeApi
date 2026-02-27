using HoroscopeApi.Application.Infrastructure.User;
using Microsoft.Extensions.DependencyInjection;

namespace HoroscopeApi.Infrastructure.User;

internal static class AddCurrentUserServiceExtensions
{
    public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}