namespace HoroscopeApi.Domain.ApiKeys;

public enum ApiKeyRateLimitType
{
    None,
    PerMinute,
    PerHour,
    PerDay
}