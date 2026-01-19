using Hermes.Results;

namespace HoroscopeApi.Domain.Horoscopes;

public static class HoroscopeErrors
{
    public static Error NotFound => new(
        "Horoscope.NotFound",
        "No horoscope found for the specified criteria.");

    public static Error InvalidPrediction => new(
        "Horoscope.InvalidPrediction",
        "General prediction is required and cannot be empty.");

    public static Error PredictionTooLong => new(
        "Horoscope.PredictionTooLong",
        "Prediction text cannot exceed 1000 characters.");

    public static Error InvalidMoodScore => new(
        "Horoscope.InvalidMoodScore",
        "Mood score must be between 1 and 10.");

    public static Error InvalidDateRange => new(
        "Horoscope.InvalidDateRange",
        "Start date must be before or equal to end date.");

    public static Error InvalidLuckyNumbers => new(
        "Horoscope.InvalidLuckyNumbers",
        "Lucky numbers are required.");

    public static Error InvalidLuckyColors => new(
        "Horoscope.InvalidLuckyColors",
        "Lucky colors are required.");

    public static Error InvalidKeywords => new(
        "Horoscope.InvalidKeywords",
        "Keywords are required.");
}
