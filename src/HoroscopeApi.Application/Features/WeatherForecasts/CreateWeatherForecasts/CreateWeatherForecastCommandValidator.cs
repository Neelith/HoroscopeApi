namespace HoroscopeApi.Application.Features.WeatherForecasts.CreateWeatherForecasts;

public class CreateWeatherForecastCommandValidator : AbstractValidator<CreateWeatherForecastCommand>
{
    public CreateWeatherForecastCommandValidator()
    {
        RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required.");
        RuleFor(x => x.TemperatureC).InclusiveBetween(-100, 100).WithMessage("Temperature must be between -100 and 100.");
        RuleFor(x => x.Summary).MaximumLength(50).WithMessage("Summary cannot exceed 50 characters.");
    }
}
