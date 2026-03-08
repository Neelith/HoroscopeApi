using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;

internal sealed class GetDailyHoroscopeValidator : AbstractValidator<GetDailyHoroscopeQuery>
{
    public GetDailyHoroscopeValidator()
    {
        RuleFor(x => x.SignName)
            .NotEmpty()
            .WithMessage("Sign name is required.")
            .Must(BeValidZodiacSign)
            .WithMessage("Invalid zodiac sign name.");
    }

    private static bool BeValidZodiacSign(string signName)
    {
        return Enum.TryParse<ZodiacSign>(signName, true, out _);
    }
}