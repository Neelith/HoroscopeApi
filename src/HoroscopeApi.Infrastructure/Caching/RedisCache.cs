using System.Text.Json;
using HoroscopeApi.Application.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Infrastructure.Caching;

internal class RedisCache(
    IDistributedCache distributedCache,
    ILogger<RedisCache> logger)
    : IRedisCache
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
        {
            logger.LogError("GetAsync called with null key.");
            throw new ArgumentNullException(nameof(key));
        }

        logger.LogDebug("Attempting to get value from cache for key: {Key}", key);

        string? value = await distributedCache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(value))
        {
            logger.LogInformation("Cache miss for key: {Key}", key);
            return default;
        }

        logger.LogInformation("Cache hit for key: {Key}", key);

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
        {
            logger.LogError("SetAsync called with null key.");
            throw new ArgumentNullException(nameof(key));
        }

        TimeSpan defaultExpiration = TimeSpan.FromMinutes(5);

        logger.LogDebug("Setting value in cache for key: {Key} with expiration: {Expiration}", key,
            expirationTime ?? defaultExpiration);

        DistributedCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = expirationTime ?? defaultExpiration
        };

        string json = JsonSerializer.Serialize(value);

        await distributedCache.SetStringAsync(key, json, options, cancellationToken);

        logger.LogInformation("Value set in cache for key: {Key}", key);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
        {
            logger.LogError("RemoveAsync called with null key.");
            throw new ArgumentNullException(nameof(key));
        }

        logger.LogDebug("Removing value from cache for key: {Key}", key);

        await distributedCache.RemoveAsync(key, cancellationToken);

        logger.LogInformation("Value removed from cache for key: {Key}", key);
    }
}