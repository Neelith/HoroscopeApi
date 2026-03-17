using HoroscopeApi.Domain.Compatibilities;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Models;

public sealed record CompatibilityData
{
    public required string FirstSign { get; init; }
    public required ZodiacSignInfoData FirstSignInfo { get; init; }
    public required string SecondSign { get; init; }
    public required ZodiacSignInfoData SecondSignInfo { get; init; }
    public required int Score { get; init; }
    public required string Description { get; init; }

    public static CompatibilityData ToCompatibilityData(Compatibility compatibility)
    {
        ZodiacSignInfo firstSignInfo = compatibility.FirstZodiacSignInfo;
        ZodiacSignInfo secondSignInfo = compatibility.SecondZodiacSignInfo;

        return new CompatibilityData
        {
            FirstSign = firstSignInfo.Sign.ToString().ToLowerInvariant(),
            FirstSignInfo = ToSignInfoData(firstSignInfo),
            SecondSign = secondSignInfo.Sign.ToString().ToLowerInvariant(),
            SecondSignInfo = ToSignInfoData(secondSignInfo),
            Score = compatibility.Score,
            Description = compatibility.Description
        };
    }

    private static ZodiacSignInfoData ToSignInfoData(ZodiacSignInfo signInfo)
    {
        return new ZodiacSignInfoData
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