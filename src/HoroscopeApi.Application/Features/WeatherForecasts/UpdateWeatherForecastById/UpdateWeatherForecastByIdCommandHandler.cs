namespace HoroscopeApi.Application.Features.WeatherForecasts.UpdateWeatherForecastById;

public sealed class UpdateWeatherForecastByIdCommandHandler()
    : ICommandHandler<UpdateWeatherForecastByIdCommand>
{
    public async Task<Result> Handle(UpdateWeatherForecastByIdCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
