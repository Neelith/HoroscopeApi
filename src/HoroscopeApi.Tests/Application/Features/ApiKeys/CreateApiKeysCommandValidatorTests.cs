using HoroscopeApi.Application.Features.ApiKeys.CreateApiKeys;

namespace HoroscopeApi.Tests.Application.Features.ApiKeys;

public sealed class CreateApiKeysCommandValidatorTests
{
    private readonly CreateApiKeysCommandValidator _validator = new();

    private static CreateApiKeyCommand ValidCommand() =>
        new(ApiKeyType.Permanent, ApiKeyRateLimitType.None);

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
    public void Validate_WithRateLimitCount_IsValid()
    {
        var command = new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.PerMinute, RateLimitCount: 100);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithRateLimitCountZero_IsInvalid()
    {
        var command = new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.PerMinute, RateLimitCount: 0);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Rate limit count must be greater than 0."));
    }

    [Fact]
    public void Validate_WithRateLimitCountNegative_IsInvalid()
    {
        var command = new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.PerMinute, RateLimitCount: -5);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithRateLimitZero_IsInvalid()
    {
        var command = new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.PerMinute, RateLimit: 0);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Rate limit must be greater than 0."));
    }

    [Fact]
    public void Validate_WithValidScopes_IsValid()
    {
        var command = new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.None, Scopes: ["read", "write"]);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyScopeName_IsInvalid()
    {
        var command = new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.None, Scopes: [""]);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Scope name cannot be empty."));
    }

    [Fact]
    public void Validate_WithNullScopes_IsValid()
    {
        var command = new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.None, Scopes: null);
        var commands = new CreateApiKeysCommands([command]);
        var result = _validator.Validate(commands);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithMultipleCommands_AllValid_IsValid()
    {
        var commands = new CreateApiKeysCommands([
            new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.None),
            new CreateApiKeyCommand(ApiKeyType.Temporary, ApiKeyRateLimitType.PerHour, RateLimitCount: 1000, RateLimit: 3600)
        ]);
        var result = _validator.Validate(commands);
        Assert.True(result.IsValid);
    }
}
