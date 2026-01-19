namespace HoroscopeApi.Application.Features.WeatherForecasts.DeleteWeatherForecastById;

public sealed class DeleteWeatherForecastByIdCommandHandler()
    : ICommandHandler<DeleteWeatherForecastByIdCommand>
{
    public async Task<Result> Handle(DeleteWeatherForecastByIdCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
