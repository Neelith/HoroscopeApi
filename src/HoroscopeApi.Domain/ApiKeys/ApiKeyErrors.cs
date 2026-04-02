using HoroscopeApi.Domain.Constants;

namespace HoroscopeApi.Domain.ApiKeys;

public static class ApiKeyErrors
{
    public static Error CreationFailed => new(
        "ApiKey.CreationFailed",
        "Failed to create one or more API keys.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.InternalServerErrorCode } }
    };

    public static Error InvalidKey => new(
        "ApiKey.InvalidKey",
        "The provided API key is invalid or expired.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.UnauthorizedCode } }
    };

    public static Error InsufficientScope => new(
        "ApiKey.InsufficientScope",
        "The API key does not have the required scope to access this resource.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.ForbiddenCode } }
    };

    public static Error RateLimitExceeded => new(
        "ApiKey.RateLimitExceeded",
        "The API key has exceeded its rate limit. Please try again later.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.TooManyRequestsCode } }
    };
}