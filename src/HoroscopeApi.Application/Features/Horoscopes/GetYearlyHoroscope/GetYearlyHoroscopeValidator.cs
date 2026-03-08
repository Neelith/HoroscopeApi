using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetYearlyHoroscope;

internal sealed class GetYearlyHoroscopeValidator : AbstractValidator<GetYearlyHoroscopeQuery>
{
    public GetYearlyHoroscopeValidator()
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