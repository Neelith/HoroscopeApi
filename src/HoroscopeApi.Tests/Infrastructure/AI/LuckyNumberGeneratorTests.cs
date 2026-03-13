using HoroscopeApi.Infrastructure.AI;

namespace HoroscopeApi.Tests.Infrastructure.AI;

public sealed class LuckyNumberGeneratorTests
{
    [Fact]
    public void Generate_ReturnsThreeNumbers()
    {
        var result = LuckyNumberGenerator.Generate(ZodiacSign.Aries, new DateOnly(2024, 6, 15));
        Assert.Equal(3, result.Length);
    }

    [Fact]
    public void Generate_ReturnsNumbersBetween1And99()
    {
        var result = LuckyNumberGenerator.Generate(ZodiacSign.Taurus, new DateOnly(2024, 3, 1));

        foreach (var number in result)
        {
            Assert.InRange(number, 1, 99);
        }
    }

    [Fact]
    public void Generate_ReturnsUniqueNumbers()
    {
        var result = LuckyNumberGenerator.Generate(ZodiacSign.Gemini, new DateOnly(2024, 5, 21));
        Assert.Equal(result.Length, result.Distinct().Count());
    }

    [Fact]
    public void Generate_SameSeedProducesSameResult()
    {
        var sign = ZodiacSign.Leo;
        var date = new DateOnly(2024, 8, 1);

        var result1 = LuckyNumberGenerator.Generate(sign, date);
        var result2 = LuckyNumberGenerator.Generate(sign, date);

        Assert.Equal(result1, result2);
    }

    [Fact]
    public void Generate_DifferentDateProducesDifferentResult()
    {
        var sign = ZodiacSign.Scorpio;
        var date1 = new DateOnly(2024, 11, 1);
        var date2 = new DateOnly(2024, 11, 2);

        var result1 = LuckyNumberGenerator.Generate(sign, date1);
        var result2 = LuckyNumberGenerator.Generate(sign, date2);

        Assert.False(result1.SequenceEqual(result2), "Different dates should (usually) produce different numbers.");
    }

    [Fact]
    public void Generate_DifferentSignProducesDifferentResult()
    {
        var date = new DateOnly(2024, 6, 1);

        var ariesResult = LuckyNumberGenerator.Generate(ZodiacSign.Aries, date);
        var taurusResult = LuckyNumberGenerator.Generate(ZodiacSign.Taurus, date);

        Assert.False(ariesResult.SequenceEqual(taurusResult), "Different signs should produce different numbers.");
    }

    [Theory]
    [InlineData(ZodiacSign.Aries)]
    [InlineData(ZodiacSign.Taurus)]
    [InlineData(ZodiacSign.Gemini)]
    [InlineData(ZodiacSign.Cancer)]
    [InlineData(ZodiacSign.Leo)]
    [InlineData(ZodiacSign.Virgo)]
    [InlineData(ZodiacSign.Libra)]
    [InlineData(ZodiacSign.Scorpio)]
    [InlineData(ZodiacSign.Sagittarius)]
    [InlineData(ZodiacSign.Capricorn)]
    [InlineData(ZodiacSign.Aquarius)]
    [InlineData(ZodiacSign.Pisces)]
    public void Generate_WorksForAllSigns(ZodiacSign sign)
    {
        var result = LuckyNumberGenerator.Generate(sign, new DateOnly(2024, 1, 1));
        Assert.Equal(3, result.Length);
        Assert.All(result, n => Assert.InRange(n, 1, 99));
    }
}
