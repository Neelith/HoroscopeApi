namespace HoroscopeApi.Tests.Domain.ZodiacSigns;

public sealed class ZodiacSignInfoCreateTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        var result = ZodiacSignInfo.Create(
            ZodiacSign.Aries, "Aries", "♈",
            3, 21, 4, 19,
            Element.Fire, Quality.Cardinal, Polarity.Positive,
            "Mars", "The first sign of the zodiac.");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(ZodiacSign.Aries, result.Value!.Sign);
        Assert.Equal("Aries", result.Value.Name);
        Assert.Equal("♈", result.Value.Symbol);
        Assert.Equal(3, result.Value.StartMonth);
        Assert.Equal(21, result.Value.StartDay);
        Assert.Equal(4, result.Value.EndMonth);
        Assert.Equal(19, result.Value.EndDay);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ReturnsFailure(string name)
    {
        var result = ZodiacSignInfo.Create(
            ZodiacSign.Taurus, name, "♉",
            4, 20, 5, 20,
            Element.Earth, Quality.Fixed, Polarity.Negative,
            "Venus", "Description.");

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptySymbol_ReturnsFailure(string symbol)
    {
        var result = ZodiacSignInfo.Create(
            ZodiacSign.Gemini, "Gemini", symbol,
            5, 21, 6, 20,
            Element.Air, Quality.Mutable, Polarity.Positive,
            "Mercury", "Description.");

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidSymbol", result.Errors[0].Code);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(13, 1)]
    [InlineData(1, 0)]
    public void Create_WithInvalidStartMonth_ReturnsFailure(int startMonth, int endMonth)
    {
        var result = ZodiacSignInfo.Create(
            ZodiacSign.Cancer, "Cancer", "♋",
            startMonth, 21, endMonth, 22,
            Element.Water, Quality.Cardinal, Polarity.Negative,
            "Moon", "Description.");

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidDateRange", result.Errors[0].Code);
    }

    [Theory]
    [InlineData(1, 13)]
    [InlineData(1, 0)]
    public void Create_WithInvalidEndMonth_ReturnsFailure(int startMonth, int endMonth)
    {
        var result = ZodiacSignInfo.Create(
            ZodiacSign.Leo, "Leo", "♌",
            startMonth, 23, endMonth, 22,
            Element.Fire, Quality.Fixed, Polarity.Positive,
            "Sun", "Description.");

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidDateRange", result.Errors[0].Code);
    }

    [Fact]
    public void Create_WithCapricornYearBoundaryMonths_ReturnsSuccess()
    {
        var result = ZodiacSignInfo.Create(
            ZodiacSign.Capricorn, "Capricorn", "♑",
            12, 22, 1, 19,
            Element.Earth, Quality.Cardinal, Polarity.Negative,
            "Saturn", "Description.");

        Assert.True(result.IsSuccess);
        Assert.Equal(12, result.Value!.StartMonth);
        Assert.Equal(1, result.Value.EndMonth);
    }
}
