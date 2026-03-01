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
}