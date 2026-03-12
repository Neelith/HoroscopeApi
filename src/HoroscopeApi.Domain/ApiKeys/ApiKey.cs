using HoroscopeApi.Domain.Shared;

namespace HoroscopeApi.Domain.ApiKeys;

public class ApiKey : AuditableEntity
{
    public int Id { get; set; }
    public required Guid OwnerId { get; set; }
    public required string Prefix { get; set; }
    public required string Hash { get; set; }
    public required string Salt { get; set; }
    public required string Algorithm { get; set; }
    public required ApiKeyType Type { get; set; }
    public required ApiKeyRateLimitType RateLimitType { get; set; }
    public int? RateLimitCount { get; set; }
    public int? RateLimit { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }

    //navigation property
    public List<ApiKeyScope> Scopes { get; set; } = [];
}