using HoroscopeApi.Domain.Constants;

namespace HoroscopeApi.Application.Services.AI;

public static class AiErrors
{
    public static Error GenerationFailed => new(
        "AI.GenerationFailed",
        "Failed to generate horoscope predictions.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode } }
    };

    public static Error InvalidApiKey => new(
        "AI.InvalidApiKey",
        "Hugging Face API key is invalid or missing.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode } }
    };

    public static Error RateLimitExceeded => new(
        "AI.RateLimitExceeded",
        "Hugging Face API rate limit exceeded. Please try again later.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode } }
    };

    public static Error ModelLoading => new(
        "AI.ModelLoading",
        "AI model is currently loading. Please try again in a moment.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode } }
    };

    public static Error InvalidResponse => new(
        "AI.InvalidResponse",
        "AI generated an invalid response format.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode } }
    };

    public static Error Timeout => new(
        "AI.Timeout",
        "AI request timed out. Please try again.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode } }
    };
}