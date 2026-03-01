namespace HoroscopeApi.Application.Features.ApiKeys.GetApiKeys;

internal sealed class GetApiKeysQueryValidator : AbstractValidator<GetApiKeysQuery>
{
    public GetApiKeysQueryValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("Invalid API key type.");

        RuleFor(x => x.RateLimitType)
            .IsInEnum()
            .When(x => x.RateLimitType.HasValue)
            .WithMessage("Invalid rate limit type.");

        RuleFor(x => x.Ids)
            .Must(BeValidCommaSeparatedIds!)
            .When(x => !string.IsNullOrWhiteSpace(x.Ids))
            .WithMessage("Ids must be a comma-separated list of valid integers.");
    }

    private static bool BeValidCommaSeparatedIds(string ids)
    {
        return ids
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .All(segment => int.TryParse(segment, out _));
    }
}