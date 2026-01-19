using Microsoft.Extensions.Logging;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
using HoroscopeApi.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;

namespace HoroscopeApi.Application.Features.WeatherForecasts.CreateWeatherForecasts;

public sealed class CreateWeatherForecastCommandHandler(
    ILogger<CreateWeatherForecastCommandHandler> logger,
    IWeatherForecastRepository weatherForecastRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateWeatherForecastCommand, IdResponse<int>>
{
    public async Task<Result<IdResponse<int>>> Handle(CreateWeatherForecastCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var createWeatherForecastResult = await weatherForecastRepository.CreateWeatherForecast(new CreateWeatherForecastRepositoryCommand
            {
                Date = command.Date,
                TemperatureC = command.TemperatureC,
                Summary = command.Summary
            }, cancellationToken);

            if (createWeatherForecastResult.IsFailure)
            {
                return Result.Ko<IdResponse<int>>(createWeatherForecastResult.Errors, createWeatherForecastResult.Metadata);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return IdResponse<int>.Create(createWeatherForecastResult.Value!.Id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while creating a weather forecast with {command}.", command);
            throw;
        }
    }
}
