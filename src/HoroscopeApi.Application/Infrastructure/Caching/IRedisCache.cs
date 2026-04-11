namespace HoroscopeApi.Application.Infrastructure.Caching;

public interface IRedisCache
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task<long> IncrementAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);

    Task<string?> TryAcquireLockAsync(string key, TimeSpan lockDuration, CancellationToken cancellationToken = default);

    Task<bool> ReleaseLockAsync(string key, string lockValue, CancellationToken cancellationToken = default);
}