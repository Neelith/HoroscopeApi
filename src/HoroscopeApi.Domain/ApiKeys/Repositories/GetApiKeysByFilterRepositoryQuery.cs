namespace HoroscopeApi.Domain.ApiKeys.Repositories;

public sealed record GetApiKeysByFilterRepositoryQuery(
    ApiKeyType? Type = null,
    ApiKeyRateLimitType? RateLimitType = null,
    List<int>? Ids = null,
    List<string>? Prefixes = null);