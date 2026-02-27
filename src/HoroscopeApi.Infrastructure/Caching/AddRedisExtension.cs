using HoroscopeApi.Application.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Infrastructure.Caching;

internal static class AddRedisExtension
{
    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        RedisSettings? redisSettings,
        ILogger logger)
    {
        //Add redis only if we have a proper connection string configured
        //Otherwise, use the in-memory cache
        if (redisSettings is null || string.IsNullOrEmpty(redisSettings.ConnectionString))
        {
            logger.LogWarning("Redis settings not found or invalid. Using in-memory cache");
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;

                logger.LogInformation("Redis connection string {Url}", redisSettings.ConnectionString);

                options.InstanceName = redisSettings.KeyPrefix;
            });
        }

        services.AddSingleton<IRedisCache, RedisCache>();

        return services;
    }
}