using HoroscopeApi.Domain.ApiKeys;

namespace HoroscopeApi.Application.Features.ApiKeys.CreateApiKeys;

internal sealed class CreateApiKeysCommandValidator : AbstractValidator<CreateApiKeysCommands>
{
    public CreateApiKeysCommandValidator()
    {
        RuleFor(x => x.Commands)
            .NotEmpty()
            .WithMessage("At least one API key item is required.");

        RuleForEach(x => x.Commands).ChildRules(item =>
        {
            item.RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("API key name is required.");

            item.RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid API key type.");

            item.RuleFor(x => x.RateLimitType)
                .IsInEnum()
                .WithMessage("Invalid rate limit type.");

            item.RuleFor(x => x.RateLimit)
                .GreaterThan(0)
                .When(x => x.RateLimit.HasValue)
                .WithMessage("Rate limit must be greater than 0.");

            item.RuleFor(x => x.ExpiresAtUtc)
                .NotNull()
                .WithMessage("Expiration date is required for temporary API keys.")
                .When(x => x.Type == ApiKeyType.Temporary);

            item.RuleFor(x => x.ExpiresAtUtc)
                .Must(date => date > DateTime.UtcNow)
                .WithMessage("Expiration date must be in the future.")
                .When(x => x.ExpiresAtUtc.HasValue);

            item.RuleForEach(x => x.Scopes)
                .NotEmpty()
                .WithMessage("Scope name cannot be empty.")
                .When(x => x.Scopes is not null && x.Scopes.Count > 0);
        });
    }
}