namespace HoroscopeApi.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsQueryValidator : AbstractValidator<GetWeatherForecastsQuery>
{
    public GetWeatherForecastsQueryValidator()
    {
        RuleFor(x => x.TemperatureRangeMin)
            .GreaterThanOrEqualTo(-20)
            .LessThanOrEqualTo(55)
            .When(x => x.TemperatureRangeMin.HasValue);

        RuleFor(x => x.TemperatureRangeMax)
            .GreaterThanOrEqualTo(-20)
            .LessThanOrEqualTo(55)
            .When(x => x.TemperatureRangeMax.HasValue);
    }
}
