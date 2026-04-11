namespace HoroscopeApi.Application.Services.ApiKey;

public sealed class RateLimitExceededException(ApiKeyRateLimitResult rateLimitResult)
    : Exception("Rate limit exceeded")
{
    public ApiKeyRateLimitResult RateLimitResult { get; } = rateLimitResult;
}