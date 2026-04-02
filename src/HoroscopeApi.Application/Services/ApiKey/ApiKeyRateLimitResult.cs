namespace HoroscopeApi.Application.Services.ApiKey;

public record ApiKeyRateLimitResult(
    bool IsAllowed,
    int Limit,
    int Remaining,
    DateTime ResetAtUtc)
{
    public bool IsExceeded => !IsAllowed;
}