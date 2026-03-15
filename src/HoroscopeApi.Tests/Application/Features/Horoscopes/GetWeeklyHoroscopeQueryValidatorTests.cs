using HoroscopeApi.Application.Features.Horoscopes.GetWeeklyHoroscope;

namespace HoroscopeApi.Tests.Application.Features.Horoscopes;

public sealed class GetWeeklyHoroscopeQueryValidatorTests
{
    private readonly GetWeeklyHoroscopeQueryValidator _validator = new();

    [Theory]
    [InlineData("Leo")]
    [InlineData("leo")]
    [InlineData("Aquarius")]
    public void Validate_WithValidSignName_IsValid(string signName)
    {
        var result = _validator.Validate(new GetWeeklyHoroscopeQuery(signName));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptySignName_IsInvalid()
    {
        var result = _validator.Validate(new GetWeeklyHoroscopeQuery(""));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Sign name is required."));
    }

    [Fact]
    public void Validate_WithInvalidSignName_IsInvalid()
    {
        var result = _validator.Validate(new GetWeeklyHoroscopeQuery("Serpens"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid zodiac sign name."));
    }
}