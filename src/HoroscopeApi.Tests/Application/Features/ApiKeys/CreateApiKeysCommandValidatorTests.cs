using HoroscopeApi.Application.Features.ApiKeys.CreateApiKeys;

namespace HoroscopeApi.Tests.Application.Features.ApiKeys;

public sealed class CreateApiKeysCommandValidatorTests
{
    private readonly CreateApiKeysCommandValidator _validator = new();

    private static CreateApiKeyCommand ValidCommand() =>
        new("My API Key", ApiKeyType.Permanent, ApiKeyRateLimitType.None);

    [Fact]
    public void Validate_WithValidCommand_IsValid()
    {
        var commands = new CreateApiKeysCommands([ValidCommand()]);
        var result = _validator.Validate(commands);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyCommandsList_IsInvalid()
    {
        var commands = new CreateApiKeysCommands([]);
        var result = _validator.Validate(commands);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("At least one API key item is required."));
    }

    [Fact]
    public void Validate_WithEmptyName_IsInvalid()
    {
        var command = new CreateApiKeyCommand("", ApiKeyType.Permanent, ApiKeyRateLimitType.None);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("API key name is required."));
    }

    [Fact]
    public void Validate_WithRateLimit_IsValid()
    {
        var command = new CreateApiKeyCommand("My Key", ApiKeyType.Permanent, ApiKeyRateLimitType.PerMinute,
            100);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithRateLimitZero_IsInvalid()
    {
        var command =
            new CreateApiKeyCommand("My Key", ApiKeyType.Permanent, ApiKeyRateLimitType.PerMinute, 0);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Rate limit must be greater than 0."));
    }

    [Fact]
    public void Validate_WithValidScopes_IsValid()
    {
        var command = new CreateApiKeyCommand("My Key", ApiKeyType.Permanent, ApiKeyRateLimitType.None,
            Scopes: ["read", "write"]);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyScopeName_IsInvalid()
    {
        var command = new CreateApiKeyCommand("My Key", ApiKeyType.Permanent, ApiKeyRateLimitType.None, Scopes: [""]);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Scope name cannot be empty."));
    }

    [Fact]
    public void Validate_WithNullScopes_IsValid()
    {
        var command = new CreateApiKeyCommand("My Key", ApiKeyType.Permanent, ApiKeyRateLimitType.None, Scopes: null);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithTemporaryTypeAndNoExpiresAtUtc_IsInvalid()
    {
        var command = new CreateApiKeyCommand("My Key", ApiKeyType.Temporary, ApiKeyRateLimitType.None);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            e => e.ErrorMessage.Contains("Expiration date is required for temporary API keys."));
    }

    [Fact]
    public void Validate_WithTemporaryTypeAndExpiresAtUtcInThePast_IsInvalid()
    {
        var command = new CreateApiKeyCommand("My Key", ApiKeyType.Temporary, ApiKeyRateLimitType.None,
            ExpiresAtUtc: DateTime.UtcNow.AddDays(-1));
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Expiration date must be in the future."));
    }

    [Fact]
    public void Validate_WithTemporaryTypeAndValidExpiresAtUtc_IsValid()
    {
        var command = new CreateApiKeyCommand("My Key", ApiKeyType.Temporary, ApiKeyRateLimitType.None,
            ExpiresAtUtc: DateTime.UtcNow.AddDays(30));
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithPermanentTypeAndNoExpiresAtUtc_IsValid()
    {
        var command = new CreateApiKeyCommand("My Key", ApiKeyType.Permanent, ApiKeyRateLimitType.None);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithMultipleCommands_AllValid_IsValid()
    {
        var commands = new CreateApiKeysCommands([
            new CreateApiKeyCommand("Key 1", ApiKeyType.Permanent, ApiKeyRateLimitType.None),
            new CreateApiKeyCommand("Key 2", ApiKeyType.Temporary, ApiKeyRateLimitType.PerHour,
                3600, DateTime.UtcNow.AddDays(30))
        ]);
        var result = _validator.Validate(commands);
        Assert.True(result.IsValid);
    }
}