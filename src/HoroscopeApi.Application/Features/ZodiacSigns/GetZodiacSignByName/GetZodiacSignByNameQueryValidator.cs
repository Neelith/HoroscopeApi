namespace HoroscopeApi.Application.Features.ZodiacSigns.GetZodiacSignByName;

internal sealed class GetZodiacSignByNameQueryValidator : AbstractValidator<GetZodiacSignByNameQuery>
{
    public GetZodiacSignByNameQueryValidator()
    {
        RuleFor(x => x.SignName)
            .NotEmpty()
            .WithMessage("Sign name is required.");
    }
}