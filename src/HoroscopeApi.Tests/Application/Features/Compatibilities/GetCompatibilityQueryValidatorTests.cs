using FluentValidation.Results;
using HoroscopeApi.Application.Features.Compatibilities.GetCompatibility;

namespace HoroscopeApi.Tests.Application.Features.Compatibilities;

public sealed class GetCompatibilityQueryValidatorTests
{
    private readonly GetCompatibilityQueryValidator _validator = new();

    [Theory]
    [InlineData("Aries", "Taurus")]
    [InlineData("aries", "taurus")]
    [InlineData("ARIES", "TAURUS")]
    [InlineData("Leo", "Virgo")]
    [InlineData("Pisces", "Scorpio")]
    [InlineData("Aries", "Aries")]
    public void Validate_WithValidSignNames_IsValid(string first, string second)
    {
        ValidationResult? result = _validator.Validate(new GetCompatibilityQuery(first, second));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyFirstSignName_IsInvalid()
    {
        ValidationResult? result = _validator.Validate(new GetCompatibilityQuery("", "Taurus"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("First sign name is required."));
    }

    [Fact]
    public void Validate_WithEmptySecondSignName_IsInvalid()
    {
        ValidationResult? result = _validator.Validate(new GetCompatibilityQuery("Aries", ""));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Second sign name is required."));
    }

    [Fact]
    public void Validate_WithInvalidFirstSignName_IsInvalid()
    {
        ValidationResult? result = _validator.Validate(new GetCompatibilityQuery("Ophiuchus", "Aries"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid first zodiac sign name."));
    }

    [Fact]
    public void Validate_WithInvalidSecondSignName_IsInvalid()
    {
        ValidationResult? result = _validator.Validate(new GetCompatibilityQuery("Aries", "Ophiuchus"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid second zodiac sign name."));
    }

    [Fact]
    public void Validate_WithBothInvalidSignNames_IsInvalid()
    {
        ValidationResult? result = _validator.Validate(new GetCompatibilityQuery("Foo", "Bar"));
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 2);
    }

    [Fact]
    public void Validate_WithWhitespaceFirstSignName_IsInvalid()
    {
        ValidationResult? result = _validator.Validate(new GetCompatibilityQuery("   ", "Aries"));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithWhitespaceSecondSignName_IsInvalid()
    {
        ValidationResult? result = _validator.Validate(new GetCompatibilityQuery("Aries", "   "));
        Assert.False(result.IsValid);
    }
}