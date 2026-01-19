using HoroscopeApi.Application.Infrastructure.Caching;
using HoroscopeApi.Domain.WeatherForecasts;
using HoroscopeApi.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
using HoroscopeApi.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Queries;

namespace HoroscopeApi.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsQueryHandler(
    IWeatherForecastRepository weatherForecastRepository,
    IRedisCache redisCache)
    : IQueryHandler<GetWeatherForecastsQuery, PagedResponse<WeatherForecast>>
{
    public async Task<Result<PagedResponse<WeatherForecast>>> Handle(GetWeatherForecastsQuery? query, CancellationToken cancellationToken)
    {
        const string cacheKey = "weatherforecasts";

        var cachedForecasts = await redisCache.GetAsync<List<WeatherForecast>>(cacheKey, cancellationToken);

        if (cachedForecasts is not null)
        {
            return PagedResponse<WeatherForecast>.Create(cachedForecasts, cachedForecasts.Count);
        }

        var getWeatherForecastsResult = await weatherForecastRepository.GetWeatherForecasts(new GetWeatherForecastsRepositoryQuery
        {
            TemperatureRangeMin = query?.TemperatureRangeMin,
            TemperatureRangeMax = query?.TemperatureRangeMax
        }, cancellationToken);

        if (getWeatherForecastsResult.IsFailure)
        {
            return Result.Ko<PagedResponse<WeatherForecast>>(getWeatherForecastsResult.Errors, getWeatherForecastsResult.Metadata);
        }

        var forecasts = getWeatherForecastsResult.Value;

        var response = PagedResponse<WeatherForecast>.Create(forecasts, forecasts.Count);

        await redisCache.SetAsync(cacheKey, forecasts, TimeSpan.FromMinutes(2), cancellationToken);

        return response;
    }
}
