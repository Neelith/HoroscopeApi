using HoroscopeApi.Domain.ApiKeys;

namespace HoroscopeApi.Application.Models;

public sealed record CreateApiKeyData
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Prefix { get; init; }
    public required string PlainTextKey { get; init; }
    public required string Type { get; init; }
    public required string RateLimitType { get; init; }
    public int? RateLimit { get; init; }
    public DateTime? ExpiresAtUtc { get; init; }
    public required List<string> Scopes { get; init; }

    public static CreateApiKeyData ToData(ApiKey apiKey, string plainTextKey)
    {
        return new CreateApiKeyData
        {
            Id = apiKey.Id,
            Name = apiKey.Name,
            Prefix = apiKey.Prefix,
            PlainTextKey = plainTextKey,
            Type = apiKey.Type.ToString(),
            RateLimitType = apiKey.RateLimitType.ToString(),
            RateLimit = apiKey.RateLimit,
            ExpiresAtUtc = apiKey.ExpiresAtUtc,
            Scopes = apiKey.Scopes.Select(s => s.Name).ToList()
        };
    }
}