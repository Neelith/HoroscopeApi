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
    private const string LockValuePrefix = "lock:";

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
            logger.LogDebug("Cache miss for key: {Key}", key);
            return default;
        }

        logger.LogDebug("Cache hit for key: {Key}", key);

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

        logger.LogDebug("Value set in cache for key: {Key}", key);
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

        logger.LogDebug("Value removed from cache for key: {Key}", key);
    }

    public async Task<long> IncrementAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        string? currentValue = await distributedCache.GetStringAsync(key, cancellationToken);
        long newValue = 1;
        if (currentValue is not null && long.TryParse(currentValue, out long existingValue))
        {
            newValue = existingValue + 1;
        }

        await distributedCache.SetStringAsync(key, newValue.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        }, cancellationToken);

        return newValue;
    }

    public async Task<string?> TryAcquireLockAsync(string key, TimeSpan lockDuration, CancellationToken cancellationToken = default)
    {
        string lockValue = $"{LockValuePrefix}{Guid.NewGuid()}";
        
        DistributedCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = lockDuration
        };

        string? existing = await distributedCache.GetStringAsync(key, cancellationToken);
        if (existing is not null)
        {
            return null;
        }

        try
        {
            await distributedCache.SetStringAsync(key, lockValue, options, cancellationToken);
            
            string? setValue = await distributedCache.GetStringAsync(key, cancellationToken);
            if (setValue == lockValue)
            {
                return lockValue;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> ReleaseLockAsync(string key, string lockValue, CancellationToken cancellationToken = default)
    {
        string? currentValue = await distributedCache.GetStringAsync(key, cancellationToken);
        
        if (currentValue == lockValue)
        {
            await distributedCache.RemoveAsync(key, cancellationToken);
            return true;
        }
        
        return false;
    }
}