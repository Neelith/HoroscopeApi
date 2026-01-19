namespace HoroscopeApi.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;

public class CreateWeatherForecastRepositoryCommand
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}
