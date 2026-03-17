using HoroscopeApi.Domain.Compatibilities;

namespace HoroscopeApi.Tests.Domain.Compatibilities;

public sealed class CompatibilityCreateTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        Result<Compatibility> result = Compatibility.Create(1, 2, 75, "Great compatibility between these signs.");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value!.FirstZodiacSignId);
        Assert.Equal(2, result.Value.SecondZodiacSignId);
        Assert.Equal(75, result.Value.Score);
        Assert.Equal("Great compatibility between these signs.", result.Value.Description);
    }

    [Fact]
    public void Create_WithScoreAtLowerBoundary_ReturnsSuccess()
    {
        Result<Compatibility> result = Compatibility.Create(1, 2, 0, "Minimal compatibility.");

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value!.Score);
    }

    [Fact]
    public void Create_WithScoreAtUpperBoundary_ReturnsSuccess()
    {
        Result<Compatibility> result = Compatibility.Create(1, 2, 100, "Perfect compatibility.");

        Assert.True(result.IsSuccess);
        Assert.Equal(100, result.Value!.Score);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(-50)]
    [InlineData(200)]
    public void Create_WithInvalidScore_ReturnsFailure(int score)
    {
        Result<Compatibility> result = Compatibility.Create(1, 2, score, "Some description.");

        Assert.True(result.IsFailure);
        Assert.Equal("Compatibility.InvalidScore", result.Errors[0].Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyDescription_ReturnsFailure(string description)
    {
        Result<Compatibility> result = Compatibility.Create(1, 2, 50, description);

        Assert.True(result.IsFailure);
        Assert.Equal("Compatibility.InvalidDescription", result.Errors[0].Code);
    }

    [Fact]
    public void Create_WithNullDescription_ReturnsFailure()
    {
        Result<Compatibility> result = Compatibility.Create(1, 2, 50, null!);

        Assert.True(result.IsFailure);
        Assert.Equal("Compatibility.InvalidDescription", result.Errors[0].Code);
    }

    [Fact]
    public void Create_WithSameSignIds_ReturnsSuccess()
    {
        Result<Compatibility> result = Compatibility.Create(1, 1, 100, "Same sign compatibility.");

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.FirstZodiacSignId);
        Assert.Equal(1, result.Value.SecondZodiacSignId);
    }
}