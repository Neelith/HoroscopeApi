using HoroscopeApi.Application.Features.ZodiacSigns.GetZodiacSignByName;

namespace HoroscopeApi.Tests.Application.Features.ZodiacSigns;

public sealed class GetZodiacSignByNameQueryValidatorTests
{
    private readonly GetZodiacSignByNameQueryValidator _validator = new();

    [Theory]
    [InlineData("Aries")]
    [InlineData("Taurus")]
    [InlineData("SomeName")]
    public void Validate_WithNonEmptySignName_IsValid(string signName)
    {
        var result = _validator.Validate(new GetZodiacSignByNameQuery(signName));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptySignName_IsInvalid()
    {
        var result = _validator.Validate(new GetZodiacSignByNameQuery(""));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Sign name is required."));
    }

    [Fact]
    public void Validate_WithWhitespaceSignName_IsInvalid()
    {
        var result = _validator.Validate(new GetZodiacSignByNameQuery("   "));
        Assert.False(result.IsValid);
    }
}
