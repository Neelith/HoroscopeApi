using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Compatibilities.GetCompatibility;

internal sealed class GetCompatibilityQueryValidator : AbstractValidator<GetCompatibilityQuery>
{
    public GetCompatibilityQueryValidator()
    {
        RuleFor(x => x.FirstSignName)
            .NotEmpty()
            .WithMessage("First sign name is required.")
            .Must(BeValidZodiacSign)
            .WithMessage("Invalid first zodiac sign name.");

        RuleFor(x => x.SecondSignName)
            .NotEmpty()
            .WithMessage("Second sign name is required.")
            .Must(BeValidZodiacSign)
            .WithMessage("Invalid second zodiac sign name.");
    }

    private static bool BeValidZodiacSign(string signName)
    {
        return Enum.TryParse<ZodiacSign>(signName, true, out _);
    }
}