using HoroscopeApi.Shared.Time;
using Microsoft.Extensions.DependencyInjection;

namespace HoroscopeApi.Infrastructure.Time;

internal static class AddTimeExtensions
{
    public static IServiceCollection AddTime(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        return services;
    }
}