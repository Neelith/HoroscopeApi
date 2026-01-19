using HoroscopeApi.Domain.WeatherForecasts;

namespace HoroscopeApi.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsQuery : IQuery<PagedResponse<WeatherForecast>>
{
    public int? TemperatureRangeMin { get; set; }
    public int? TemperatureRangeMax { get; set; }
}
