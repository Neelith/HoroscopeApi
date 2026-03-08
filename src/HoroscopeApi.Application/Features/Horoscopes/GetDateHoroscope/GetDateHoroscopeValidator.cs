using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDateHoroscope;

internal sealed class GetDateHoroscopeValidator : AbstractValidator<GetDateHoroscopeQuery>
{
    public GetDateHoroscopeValidator()
    {
        RuleFor(x => x.SignName)
            .NotEmpty()
            .WithMessage("Sign name is required.")
            .Must(BeValidZodiacSign)
            .WithMessage("Invalid zodiac sign name.");

        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)))
            .WithMessage("Date cannot be more than 1 year in the future.");
    }

    private static bool BeValidZodiacSign(string signName)
    {
        return Enum.TryParse<ZodiacSign>(signName, true, out _);
    }
}