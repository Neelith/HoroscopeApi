using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetWeeklyHoroscope;

internal sealed class GetWeeklyHoroscopeValidator : AbstractValidator<GetWeeklyHoroscopeRequest>
{
    public GetWeeklyHoroscopeValidator()
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
