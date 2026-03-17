using HoroscopeApi.Application.Services.CompatibilityService;
using HoroscopeApi.Application.Services.HoroscopeService;
using HoroscopeApi.Infrastructure.ApiKey;
using HoroscopeApi.Infrastructure.Caching;
using HoroscopeApi.Infrastructure.CompatibilityService;
using HoroscopeApi.Infrastructure.HoroscopeService;
using HoroscopeApi.Infrastructure.Persistence;
using HoroscopeApi.Infrastructure.Persistence.Repositories;
using HoroscopeApi.Infrastructure.Time;
using HoroscopeApi.Infrastructure.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        ILogger logger,
        string? dbConnectionString,
        RedisSettings? redisSettings = default)
    {
        //Register infrastructure services here
        ArgumentNullException.ThrowIfNull(dbConnectionString);

        services.AddTime()
            .AddDbContext(dbConnectionString)
            .AddRepositories()
            .AddRedis(redisSettings, logger)
            .AddCurrentUserService()
            .AddApiKeyServices()
            .AddScoped<IHoroscopeQueryService, HoroscopeQueryService>()
            .AddScoped<ICompatibilityQueryService, CompatibilityQueryService>();

        return services;
    }
}