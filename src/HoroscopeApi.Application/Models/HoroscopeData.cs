using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Models;

public sealed record HoroscopeData
{
    public required ZodiacSignData ZodiacSign { get; init; }
    public required string Period { get; init; }
    public required DateOnly Date { get; init; }
    public required HoroscopePredictions Predictions { get; init; }
    public required List<int> LuckyNumbers { get; init; }
    public required List<string> LuckyColors { get; init; }
    public required int MoodScore { get; init; }
    public required List<string> Keywords { get; init; }

    public static HoroscopeData ToHoroscopeData(Horoscope horoscope)
    {
        ZodiacSignInfo signInfo = horoscope.ZodiacSignInfo;

        return new HoroscopeData
        {
            ZodiacSign = new ZodiacSignData
            {
                Name = signInfo.Name,
                Symbol = signInfo.Symbol,
                Element = signInfo.Element.ToString(),
                Quality = signInfo.Quality.ToString(),
                Polarity = signInfo.Polarity.ToString(),
                RulingPlanet = signInfo.RulingPlanet,
                DateRange =
                    GetDateRangeString(signInfo.StartMonth, signInfo.StartDay, signInfo.EndMonth,
                        signInfo.EndDay),
                Description = signInfo.Description
            },
            Period = horoscope.Period.ToString().ToLowerInvariant(),
            Date = horoscope.Date,
            Predictions = new HoroscopePredictions
            {
                General = horoscope.GeneralPrediction,
                Love = horoscope.LovePrediction,
                Career = horoscope.CareerPrediction,
                Health = horoscope.HealthPrediction
            },
            LuckyNumbers = horoscope.LuckyNumbers,
            LuckyColors = horoscope.LuckyColors,
            MoodScore = horoscope.MoodScore,
            Keywords = horoscope.Keywords
        };
    }

    private static string GetDateRangeString(int startMonth, int startDay, int endMonth, int endDay)
    {
        string startMonthName = new DateOnly(2000, startMonth, 1).ToString("MMMM");
        string endMonthName = new DateOnly(2000, endMonth, 1).ToString("MMMM");
        return $"{startMonthName} {startDay} - {endMonthName} {endDay}";
    }
}