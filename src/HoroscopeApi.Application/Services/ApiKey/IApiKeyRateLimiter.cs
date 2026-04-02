using HoroscopeApi.Domain.ApiKeys;

namespace HoroscopeApi.Application.Services.ApiKey;

public interface IApiKeyRateLimiter
{
    Task<ApiKeyRateLimitResult> CheckRateLimitAsync(
        int apiKeyId,
        ApiKeyRateLimitType rateLimitType,
        int limit,
        CancellationToken cancellationToken);
}