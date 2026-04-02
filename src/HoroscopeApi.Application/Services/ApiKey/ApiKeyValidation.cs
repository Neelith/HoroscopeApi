using HoroscopeApi.Domain.ApiKeys;

namespace HoroscopeApi.Application.Services.ApiKey;

public record ApiKeyValidation(
    bool IsValid,
    int? ApiKeyId = null,
    ApiKeyRateLimitType? RateLimitType = null,
    int? RateLimit = null,
    List<string>? Scopes = null)
{
    public bool IsNotValid => !IsValid;
}