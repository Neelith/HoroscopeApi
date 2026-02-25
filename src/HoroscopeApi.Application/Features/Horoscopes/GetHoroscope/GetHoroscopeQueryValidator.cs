using FluentValidation;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetHoroscope;

internal sealed class GetHoroscopeQueryValidator : AbstractValidator<GetHoroscopeQuery>
{
    public GetHoroscopeQueryValidator()
    {
        RuleFor(x => x.SignName)
            .NotEmpty()
            .WithMessage("Sign name is required.")
            .Must(BeValidZodiacSign)
            .WithMessage("Invalid zodiac sign name.");

        // Period validation - only validate if Date is not provided
        RuleFor(x => x.Period)
            .IsInEnum()
            .When(x => x.Period.HasValue && !x.Date.HasValue)
            .WithMessage("Invalid horoscope period.");

        // Date validation - validate when Date is provided
        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)))
            .When(x => x.Date.HasValue)
            .WithMessage("Date cannot be more than 1 year in the future.");
    }

    private static bool BeValidZodiacSign(string signName)
    {
        return Enum.TryParse<ZodiacSign>(signName, true, out _);
    }
}
