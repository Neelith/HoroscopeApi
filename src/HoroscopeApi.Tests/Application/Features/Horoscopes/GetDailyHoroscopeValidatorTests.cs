using HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;

namespace HoroscopeApi.Tests.Application.Features.Horoscopes;

public sealed class GetDailyHoroscopeValidatorTests
{
    private readonly GetDailyHoroscopeValidator _validator = new();

    [Theory]
    [InlineData("Aries")]
    [InlineData("aries")]
    [InlineData("ARIES")]
    [InlineData("Taurus")]
    [InlineData("Gemini")]
    [InlineData("Cancer")]
    [InlineData("Leo")]
    [InlineData("Virgo")]
    [InlineData("Libra")]
    [InlineData("Scorpio")]
    [InlineData("Sagittarius")]
    [InlineData("Capricorn")]
    [InlineData("Aquarius")]
    [InlineData("Pisces")]
    public void Validate_WithValidSignName_IsValid(string signName)
    {
        var result = _validator.Validate(new GetDailyHoroscopeQuery(signName));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptySignName_IsInvalid()
    {
        var result = _validator.Validate(new GetDailyHoroscopeQuery(""));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Sign name is required."));
    }

    [Fact]
    public void Validate_WithInvalidSignName_IsInvalid()
    {
        var result = _validator.Validate(new GetDailyHoroscopeQuery("Ophiuchus"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid zodiac sign name."));
    }

    [Fact]
    public void Validate_WithWhitespaceSignName_IsInvalid()
    {
        var result = _validator.Validate(new GetDailyHoroscopeQuery("   "));
        Assert.False(result.IsValid);
    }
}
