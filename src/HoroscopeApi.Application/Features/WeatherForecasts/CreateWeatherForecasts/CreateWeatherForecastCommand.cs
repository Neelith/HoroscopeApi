namespace HoroscopeApi.Application.Features.WeatherForecasts.CreateWeatherForecasts;

public class CreateWeatherForecastCommand : ICommand<IdResponse<int>>
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}
