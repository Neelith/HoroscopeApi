using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Domain.ApiKeys;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Infrastructure.ApiKey;

internal class RedisApiKeyRateLimiter(
    IDistributedCache distributedCache,
    IDateTimeProvider dateTimeProvider,
    ILogger<RedisApiKeyRateLimiter> logger) : IApiKeyRateLimiter
{
    public async Task<ApiKeyRateLimitResult> CheckRateLimitAsync(
        int apiKeyId,
        ApiKeyRateLimitType rateLimitType,
        int limit,
        CancellationToken cancellationToken)
    {
        DateTime utcNow = dateTimeProvider.UtcNow;
        (string windowKey, TimeSpan windowDuration, DateTime resetAtUtc) =
            ComputeWindow(apiKeyId, rateLimitType, utcNow);

        logger.LogDebug(
            "Checking rate limit for API key {ApiKeyId}, window key: {WindowKey}, limit: {Limit}",
            apiKeyId, windowKey, limit);

        int currentCount = await IncrementCounterAsync(windowKey, windowDuration, cancellationToken: cancellationToken);

        bool isAllowed = currentCount <= limit;
        int remaining = Math.Max(0, limit - currentCount);

        if (!isAllowed)
        {
            logger.LogWarning(
                "Rate limit exceeded for API key {ApiKeyId}. Count: {Count}, Limit: {Limit}, Resets at: {ResetAtUtc}",
                apiKeyId, currentCount, limit, resetAtUtc.ToString("o"));
        }

        return new ApiKeyRateLimitResult(isAllowed, limit, remaining, resetAtUtc);
    }

    internal static (string WindowKey, TimeSpan WindowDuration, DateTime ResetAtUtc) ComputeWindow(
        int apiKeyId,
        ApiKeyRateLimitType rateLimitType,
        DateTime utcNow)
    {
        return rateLimitType switch
        {
            ApiKeyRateLimitType.PerMinute => (
                $"ratelimit:{apiKeyId}:{utcNow:yyyy-MM-ddTHH:mm}",
                TimeSpan.FromSeconds(60),
                new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, 0, DateTimeKind.Utc)
                    .AddMinutes(1)),

            ApiKeyRateLimitType.PerHour => (
                $"ratelimit:{apiKeyId}:{utcNow:yyyy-MM-ddTHH}",
                TimeSpan.FromSeconds(3600),
                new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, 0, 0, DateTimeKind.Utc)
                    .AddHours(1)),

            ApiKeyRateLimitType.PerDay => (
                $"ratelimit:{apiKeyId}:{utcNow:yyyy-MM-dd}",
                TimeSpan.FromSeconds(86400),
                new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc)
                    .AddDays(1)),

            _ => throw new ArgumentOutOfRangeException(nameof(rateLimitType),
                $"Rate limit type '{rateLimitType}' is not supported for rate limiting.")
        };
    }

    private async Task<int> IncrementCounterAsync(
        string windowKey,
        TimeSpan windowDuration,
        int maxRetries = 3,
        CancellationToken cancellationToken = default)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            string? currentValue = await distributedCache.GetStringAsync(windowKey, cancellationToken);
            long newCount = currentValue == null ? 1 : long.Parse(currentValue) + 1;

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = windowDuration
            };

            if (currentValue == null)
            {
                try
                {
                    await distributedCache.SetStringAsync(windowKey, newCount.ToString(), options, cancellationToken);
                    return (int)newCount;
                }
                catch (Exception) when (attempt < maxRetries - 1)
                {
                    continue;
                }
            }

            try
            {
                await distributedCache.SetStringAsync(windowKey, newCount.ToString(), options, cancellationToken);
                return (int)newCount;
            }
            catch (Exception) when (attempt < maxRetries - 1)
            {
                continue;
            }
        }

        return 1;
    }
}