namespace HoroscopeApi.Domain.ApiKeys.Repositories;

public sealed record GetApiKeysByFilterRepositoryQuery(
    Guid? OwnerId = null,
    ApiKeyType? Type = null,
    ApiKeyRateLimitType? RateLimitType = null,
    List<int>? Ids = null,
    List<string>? Prefixes = null);