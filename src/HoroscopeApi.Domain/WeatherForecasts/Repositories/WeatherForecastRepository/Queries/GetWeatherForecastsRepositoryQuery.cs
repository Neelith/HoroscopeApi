namespace HoroscopeApi.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Queries;

public class GetWeatherForecastsRepositoryQuery
{
    public int? TemperatureRangeMin { get; set; }
    public int? TemperatureRangeMax { get; set; }
}
