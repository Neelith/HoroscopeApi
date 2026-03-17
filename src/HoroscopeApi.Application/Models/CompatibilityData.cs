using HoroscopeApi.Domain.Compatibilities;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Models;

public sealed record CompatibilityData
{
    public required ZodiacSignData FirstZodiacSign { get; init; }
    public required ZodiacSignData SecondZodiacSign { get; init; }
    public required int Score { get; init; }
    public required string Description { get; init; }

    public static CompatibilityData ToCompatibilityData(Compatibility compatibility)
    {
        ZodiacSignInfo firstSignInfo = compatibility.FirstZodiacSignInfo;
        ZodiacSignInfo secondSignInfo = compatibility.SecondZodiacSignInfo;

        return new CompatibilityData
        {
            FirstZodiacSign = ToZodiacSignData(firstSignInfo),
            SecondZodiacSign = ToZodiacSignData(secondSignInfo),
            Score = compatibility.Score,
            Description = compatibility.Description
        };
    }

    private static ZodiacSignData ToZodiacSignData(ZodiacSignInfo signInfo)
    {
        return new ZodiacSignData
        {
            Name = signInfo.Name,
            Symbol = signInfo.Symbol,
            Element = signInfo.Element.ToString(),
            Quality = signInfo.Quality.ToString(),
            Polarity = signInfo.Polarity.ToString(),
            RulingPlanet = signInfo.RulingPlanet,
            DateRange = GetDateRangeString(signInfo.StartMonth, signInfo.StartDay, signInfo.EndMonth,
                signInfo.EndDay),
            Description = signInfo.Description
        };
    }

    private static string GetDateRangeString(int startMonth, int startDay, int endMonth, int endDay)
    {
        string startMonthName = new DateOnly(2000, startMonth, 1).ToString("MMMM");
        string endMonthName = new DateOnly(2000, endMonth, 1).ToString("MMMM");
        return $"{startMonthName} {startDay} - {endMonthName} {endDay}";
    }
}