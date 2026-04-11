namespace HoroscopeApi.WebApi.Constants;

public static class Headers
{
    public const string TraceParent = "traceparent";
    public const string TraceState = "tracestate";
    public const string Correlation = "x-correlation-id";
    public const string ApiKey = "x-api-key";
    public const string RateLimitLimit = "X-RateLimit-Limit";
    public const string RateLimitRemaining = "X-RateLimit-Remaining";
    public const string RateLimitReset = "X-RateLimit-Reset";
    public const string RetryAfter = "Retry-After";
}