using Hermes.Results;
using HoroscopeApi.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;
using HoroscopeApi.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Queries;

namespace HoroscopeApi.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

public interface IWeatherForecastRepository
{
    Task<Result<List<WeatherForecast>>> GetWeatherForecasts(GetWeatherForecastsRepositoryQuery? query, CancellationToken cancellationToken);
    Task<Result<WeatherForecast>> CreateWeatherForecast(CreateWeatherForecastRepositoryCommand command, CancellationToken cancellationToken);
}
