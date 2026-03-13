using HoroscopeApi.Application.Features.Horoscopes.GetDateHoroscope;

namespace HoroscopeApi.Tests.Application.Features.Horoscopes;

public sealed class GetDateHoroscopeValidatorTests
{
    private readonly GetDateHoroscopeValidator _validator = new();

    [Fact]
    public void Validate_WithValidSignAndPastDate_IsValid()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var result = _validator.Validate(new GetDateHoroscopeQuery("Aries", date));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithValidSignAndTodaysDate_IsValid()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = _validator.Validate(new GetDateHoroscopeQuery("Taurus", date));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithValidSignAndDateWithinOneYear_IsValid()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6));
        var result = _validator.Validate(new GetDateHoroscopeQuery("Gemini", date));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithDateMoreThanOneYearInFuture_IsInvalid()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1).AddDays(2));
        var result = _validator.Validate(new GetDateHoroscopeQuery("Aries", date));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Date cannot be more than 1 year in the future."));
    }

    [Fact]
    public void Validate_WithEmptySignName_IsInvalid()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = _validator.Validate(new GetDateHoroscopeQuery("", date));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Sign name is required."));
    }

    [Fact]
    public void Validate_WithInvalidSignName_IsInvalid()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = _validator.Validate(new GetDateHoroscopeQuery("NotASign", date));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid zodiac sign name."));
    }
}
