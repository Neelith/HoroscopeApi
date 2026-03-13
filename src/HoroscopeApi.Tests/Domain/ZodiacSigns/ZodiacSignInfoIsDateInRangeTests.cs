namespace HoroscopeApi.Tests.Domain.ZodiacSigns;

public sealed class ZodiacSignInfoIsDateInRangeTests
{
    private static ZodiacSignInfo CreateSign(int startMonth, int startDay, int endMonth, int endDay,
        ZodiacSign sign = ZodiacSign.Aries)
        => ZodiacSignInfo.Create(sign, "TestSign", "♈",
            startMonth, startDay, endMonth, endDay,
            Element.Fire, Quality.Cardinal, Polarity.Positive,
            "Mars", "Test").Value!;

    [Fact]
    public void IsDateInRange_DateWithinRange_ReturnsTrue()
    {
        var sign = CreateSign(3, 21, 4, 19);
        Assert.True(sign.IsDateInRange(4, 1));
    }

    [Fact]
    public void IsDateInRange_DateOnStartBoundary_ReturnsTrue()
    {
        var sign = CreateSign(3, 21, 4, 19);
        Assert.True(sign.IsDateInRange(3, 21));
    }

    [Fact]
    public void IsDateInRange_DateOnEndBoundary_ReturnsTrue()
    {
        var sign = CreateSign(3, 21, 4, 19);
        Assert.True(sign.IsDateInRange(4, 19));
    }

    [Fact]
    public void IsDateInRange_DateBeforeStartMonth_ReturnsFalse()
    {
        var sign = CreateSign(3, 21, 4, 19);
        Assert.False(sign.IsDateInRange(2, 28));
    }

    [Fact]
    public void IsDateInRange_DateAfterEndMonth_ReturnsFalse()
    {
        var sign = CreateSign(3, 21, 4, 19);
        Assert.False(sign.IsDateInRange(5, 1));
    }

    [Fact]
    public void IsDateInRange_DateBeforeStartDay_ReturnsFalse()
    {
        var sign = CreateSign(3, 21, 4, 19);
        Assert.False(sign.IsDateInRange(3, 20));
    }

    [Fact]
    public void IsDateInRange_DateAfterEndDay_ReturnsFalse()
    {
        var sign = CreateSign(3, 21, 4, 19);
        Assert.False(sign.IsDateInRange(4, 20));
    }

    [Fact]
    public void IsDateInRange_CapricornDecemberDate_ReturnsTrue()
    {
        var sign = CreateSign(12, 22, 1, 19, ZodiacSign.Capricorn);
        Assert.True(sign.IsDateInRange(12, 25));
    }

    [Fact]
    public void IsDateInRange_CapricornJanuaryDate_ReturnsTrue()
    {
        var sign = CreateSign(12, 22, 1, 19, ZodiacSign.Capricorn);
        Assert.True(sign.IsDateInRange(1, 10));
    }

    [Fact]
    public void IsDateInRange_CapricornDecemberBeforeStart_ReturnsFalse()
    {
        var sign = CreateSign(12, 22, 1, 19, ZodiacSign.Capricorn);
        Assert.False(sign.IsDateInRange(12, 21));
    }

    [Fact]
    public void IsDateInRange_CapricornJanuaryAfterEnd_ReturnsFalse()
    {
        var sign = CreateSign(12, 22, 1, 19, ZodiacSign.Capricorn);
        Assert.False(sign.IsDateInRange(1, 20));
    }

    [Fact]
    public void IsDateInRange_CapricornMidYearDate_ReturnsFalse()
    {
        var sign = CreateSign(12, 22, 1, 19, ZodiacSign.Capricorn);
        Assert.False(sign.IsDateInRange(6, 15));
    }
}
