using HoroscopeApi.Application.Features.Horoscopes.GetYearlyHoroscope;

namespace HoroscopeApi.Tests.Application.Features.Horoscopes;

public sealed class GetYearlyHoroscopeValidatorTests
{
    private readonly GetYearlyHoroscopeValidator _validator = new();

    [Theory]
    [InlineData("Sagittarius")]
    [InlineData("sagittarius")]
    [InlineData("Capricorn")]
    public void Validate_WithValidSignName_IsValid(string signName)
    {
        var result = _validator.Validate(new GetYearlyHoroscopeQuery(signName));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptySignName_IsInvalid()
    {
        var result = _validator.Validate(new GetYearlyHoroscopeQuery(""));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Sign name is required."));
    }

    [Fact]
    public void Validate_WithInvalidSignName_IsInvalid()
    {
        var result = _validator.Validate(new GetYearlyHoroscopeQuery("Galaxy"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid zodiac sign name."));
    }

    [Fact]
    public void Validate_WithOptionalYear_IsValid()
    {
        var result = _validator.Validate(new GetYearlyHoroscopeQuery("Aries", 2025));
        Assert.True(result.IsValid);
    }
}
