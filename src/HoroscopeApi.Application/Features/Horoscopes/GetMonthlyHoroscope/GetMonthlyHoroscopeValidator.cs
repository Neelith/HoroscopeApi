using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetMonthlyHoroscope;

internal sealed class GetMonthlyHoroscopeValidator : AbstractValidator<GetMonthlyHoroscopeQuery>
{
    public GetMonthlyHoroscopeValidator()
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