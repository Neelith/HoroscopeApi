using FluentValidation;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;

internal sealed class GetDailyHoroscopeQueryValidator : AbstractValidator<GetDailyHoroscopeQuery>
{
    public GetDailyHoroscopeQueryValidator()
    {
        RuleFor(x => x.SignName)
            .NotEmpty()
            .WithMessage("Sign name is required.");

        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)))
            .When(x => x.Date.HasValue)
            .WithMessage("Date cannot be more than 1 year in the future.");
    }
}
