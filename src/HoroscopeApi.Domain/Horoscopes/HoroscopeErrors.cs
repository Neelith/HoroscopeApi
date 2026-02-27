using HoroscopeApi.Shared.Constants;

namespace HoroscopeApi.Domain.Horoscopes;

public static class HoroscopeErrors
{
    public static Error NotFound => new(
        "Horoscope.NotFound",
        "No horoscope found for the specified criteria.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode } }
    };

    public static Error InvalidPrediction => new(
        "Horoscope.InvalidPrediction",
        "General prediction is required and cannot be empty.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode } }
    };

    public static Error PredictionTooLong => new(
        "Horoscope.PredictionTooLong",
        "Prediction text cannot exceed 1000 characters.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode } }
    };

    public static Error InvalidMoodScore => new(
        "Horoscope.InvalidMoodScore",
        "Mood score must be between 1 and 10.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode } }
    };

    public static Error InvalidDateRange => new(
        "Horoscope.InvalidDateRange",
        "Start date must be before or equal to end date.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode } }
    };

    public static Error InvalidLuckyNumbers => new(
        "Horoscope.InvalidLuckyNumbers",
        "At least one lucky number is required.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode } }
    };

    public static Error InvalidLuckyColors => new(
        "Horoscope.InvalidLuckyColors",
        "At least one lucky color is required.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode } }
    };

    public static Error InvalidKeywords => new(
        "Horoscope.InvalidKeywords",
        "At least one keyword is required.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode } }
    };
}