using HoroscopeApi.Application.Features.ApiKeys.GetApiKeys;

namespace HoroscopeApi.Tests.Application.Features.ApiKeys;

public sealed class GetApiKeysQueryValidatorTests
{
    private readonly GetApiKeysQueryValidator _validator = new();

    [Fact]
    public void Validate_WithNoFilters_IsValid()
    {
        var result = _validator.Validate(new GetApiKeysQuery());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithValidType_IsValid()
    {
        var result = _validator.Validate(new GetApiKeysQuery(Type: ApiKeyType.Permanent));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithValidRateLimitType_IsValid()
    {
        var result = _validator.Validate(new GetApiKeysQuery(RateLimitType: ApiKeyRateLimitType.PerHour));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithValidCommaSeparatedIds_IsValid()
    {
        var result = _validator.Validate(new GetApiKeysQuery(Ids: "1,2,3"));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithSingleId_IsValid()
    {
        var result = _validator.Validate(new GetApiKeysQuery(Ids: "42"));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithNonIntegerIds_IsInvalid()
    {
        var result = _validator.Validate(new GetApiKeysQuery(Ids: "1,abc,3"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Ids must be a comma-separated list of valid integers."));
    }

    [Fact]
    public void Validate_WithNullIds_IsValid()
    {
        var result = _validator.Validate(new GetApiKeysQuery(Ids: null));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithIdsContainingSpaces_IsValid()
    {
        var result = _validator.Validate(new GetApiKeysQuery(Ids: "1, 2, 3"));
        Assert.True(result.IsValid);
    }
}
