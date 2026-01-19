namespace HoroscopeApi.Domain.WeatherForecasts;

public static class WeatherForecastErrors
{

}
public static class SummaryErrors
{
    public static Error NullOrEmpty => new(
        "Summary.NullOrEmpty",
        "Summary cannot be null or empty."
    );

    public static Error SummaryTooLong => new(
        "Summary.TooLong",
        "Summary cannot exceed 256 characters."
    );
}
