using HoroscopeApi.Application.Features.Horoscopes.GetMonthlyHoroscope;

namespace HoroscopeApi.Tests.Application.Features.Horoscopes;

public sealed class GetMonthlyHoroscopeValidatorTests
{
    private readonly GetMonthlyHoroscopeValidator _validator = new();

    [Theory]
    [InlineData("Aries")]
    [InlineData("Pisces")]
    [InlineData("scorpio")]
    public void Validate_WithValidSignName_IsValid(string signName)
    {
        var result = _validator.Validate(new GetMonthlyHoroscopeQuery(signName));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptySignName_IsInvalid()
    {
        var result = _validator.Validate(new GetMonthlyHoroscopeQuery(""));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Sign name is required."));
    }

    [Fact]
    public void Validate_WithInvalidSignName_IsInvalid()
    {
        var result = _validator.Validate(new GetMonthlyHoroscopeQuery("Unknown"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid zodiac sign name."));
    }
}
