using Microsoft.Extensions.DependencyInjection;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;

namespace HoroscopeApi.Infrastructure.Persistence.Repositories;

internal static class AddRepositoriesExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IZodiacSignRepository, ZodiacSignRepository>();
        services.AddScoped<IHoroscopeRepository, HoroscopeRepository>();

        return services;
    }
}
