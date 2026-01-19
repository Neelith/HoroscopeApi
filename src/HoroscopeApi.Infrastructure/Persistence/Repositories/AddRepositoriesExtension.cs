using Microsoft.Extensions.DependencyInjection;
using HoroscopeApi.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

namespace HoroscopeApi.Infrastructure.Persistence.Repositories;

internal static class AddRepositoriesExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();

        return services;
    }
}
