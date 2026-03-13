namespace HoroscopeApi.Tests.Domain.Horoscopes;

public sealed class HoroscopeCreateTests
{
    private static readonly List<int> ValidNumbers = [1, 2, 3];
    private static readonly List<string> ValidColors = ["red", "blue"];
    private static readonly List<string> ValidKeywords = ["growth", "energy"];
    private static readonly DateOnly ValidDate = new(2024, 6, 15);

    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        var result = Horoscope.Create(1, HoroscopePeriod.Daily, ValidDate,
            "A bright day ahead.", "Love blooms.", "Career rises.", "Stay healthy.",
            ValidNumbers, ValidColors, 7, ValidKeywords);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("A bright day ahead.", result.Value!.GeneralPrediction);
        Assert.Equal(7, result.Value.MoodScore);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyGeneralPrediction_ReturnsFailure(string prediction)
    {
        var result = Horoscope.Create(1, HoroscopePeriod.Daily, ValidDate,
            prediction, null, null, null, ValidNumbers, ValidColors, 5, ValidKeywords);

        Assert.True(result.IsFailure);
        Assert.Equal("Horoscope.InvalidPrediction", result.Errors[0].Code);
    }

    [Fact]
    public void Create_WithGeneralPredictionExceeding1000Chars_ReturnsFailure()
    {
        string longPrediction = new('x', 1001);

        var result = Horoscope.Create(1, HoroscopePeriod.Daily, ValidDate,
            longPrediction, null, null, null, ValidNumbers, ValidColors, 5, ValidKeywords);

        Assert.True(result.IsFailure);
        Assert.Equal("Horoscope.PredictionTooLong", result.Errors[0].Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(11)]
    public void Create_WithInvalidMoodScore_ReturnsFailure(int moodScore)
    {
        var result = Horoscope.Create(1, HoroscopePeriod.Daily, ValidDate,
            "Valid prediction.", null, null, null, ValidNumbers, ValidColors, moodScore, ValidKeywords);

        Assert.True(result.IsFailure);
        Assert.Equal("Horoscope.InvalidMoodScore", result.Errors[0].Code);
    }

    [Fact]
    public void Create_WithMoodScoreAtBoundary1_ReturnsSuccess()
    {
        var result = Horoscope.Create(1, HoroscopePeriod.Daily, ValidDate,
            "Valid prediction.", null, null, null, ValidNumbers, ValidColors, 1, ValidKeywords);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithMoodScoreAtBoundary10_ReturnsSuccess()
    {
        var result = Horoscope.Create(1, HoroscopePeriod.Daily, ValidDate,
            "Valid prediction.", null, null, null, ValidNumbers, ValidColors, 10, ValidKeywords);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithEmptyLuckyNumbers_ReturnsFailure()
    {
        var result = Horoscope.Create(1, HoroscopePeriod.Daily, ValidDate,
            "Valid prediction.", null, null, null, [], ValidColors, 5, ValidKeywords);

        Assert.True(result.IsFailure);
        Assert.Equal("Horoscope.InvalidLuckyNumbers", result.Errors[0].Code);
    }

    [Fact]
    public void Create_WithEmptyLuckyColors_ReturnsFailure()
    {
        var result = Horoscope.Create(1, HoroscopePeriod.Daily, ValidDate,
            "Valid prediction.", null, null, null, ValidNumbers, [], 5, ValidKeywords);

        Assert.True(result.IsFailure);
        Assert.Equal("Horoscope.InvalidLuckyColors", result.Errors[0].Code);
    }

    [Fact]
    public void Create_WithEmptyKeywords_ReturnsFailure()
    {
        var result = Horoscope.Create(1, HoroscopePeriod.Daily, ValidDate,
            "Valid prediction.", null, null, null, ValidNumbers, ValidColors, 5, []);

        Assert.True(result.IsFailure);
        Assert.Equal("Horoscope.InvalidKeywords", result.Errors[0].Code);
    }

    [Fact]
    public void Create_WithNullableOptionalFields_ReturnsSuccess()
    {
        var result = Horoscope.Create(1, HoroscopePeriod.Weekly, ValidDate,
            "A bright week.", null, null, null, ValidNumbers, ValidColors, 8, ValidKeywords);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.LovePrediction);
        Assert.Null(result.Value.CareerPrediction);
        Assert.Null(result.Value.HealthPrediction);
    }
}
