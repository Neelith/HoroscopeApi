using HoroscopeApi.Domain.ZodiacSigns;
using HoroscopeApi.Shared.Domain;
using Hermes.Results;

namespace HoroscopeApi.Domain.Horoscopes;

public sealed class Horoscope : AuditableEntity
{
    public int Id { get; private set; }
    public int ZodiacSignId { get; private set; }
    public ZodiacSignInfo ZodiacSignInfo { get; private set; } = null!;
    public HoroscopePeriod Period { get; private set; }
    public DateOnly Date { get; private set; }
    public string GeneralPrediction { get; private set; } = string.Empty;
    public string? LovePrediction { get; private set; }
    public string? CareerPrediction { get; private set; }
    public string? HealthPrediction { get; private set; }
    public List<int> LuckyNumbers { get; private set; } = new();
    public List<string> LuckyColors { get; private set; } = new();
    public int MoodScore { get; private set; }
    public List<string> Keywords { get; private set; } = new();

    //EF constructor - required for entity materialization
    private Horoscope() { }

    private Horoscope(
        int zodiacSignId,
        HoroscopePeriod period,
        DateOnly date,
        string generalPrediction,
        string? lovePrediction,
        string? careerPrediction,
        string? healthPrediction,
        List<int> luckyNumbers,
        List<string> luckyColors,
        int moodScore,
        List<string> keywords)
    {
        ZodiacSignId = zodiacSignId;
        Period = period;
        Date = date;
        GeneralPrediction = generalPrediction;
        LovePrediction = lovePrediction;
        CareerPrediction = careerPrediction;
        HealthPrediction = healthPrediction;
        LuckyNumbers = luckyNumbers;
        LuckyColors = luckyColors;
        MoodScore = moodScore;
        Keywords = keywords;
    }

    public static Result<Horoscope> Create(
        int zodiacSignId,
        HoroscopePeriod period,
        DateOnly date,
        string generalPrediction,
        string? lovePrediction,
        string? careerPrediction,
        string? healthPrediction,
        List<int> luckyNumbers,
        List<string> luckyColors,
        int moodScore,
        List<string> keywords)
    {
        if (string.IsNullOrWhiteSpace(generalPrediction))
        {
            return Result.Ko<Horoscope>(HoroscopeErrors.InvalidPrediction);
        }

        if (generalPrediction.Length > 1000)
        {
            return Result.Ko<Horoscope>(HoroscopeErrors.PredictionTooLong);
        }

        if (moodScore < 1 || moodScore > 10)
        {
            return Result.Ko<Horoscope>(HoroscopeErrors.InvalidMoodScore);
        }

        if (luckyNumbers == null || luckyNumbers.Count == 0)
        {
            return Result.Ko<Horoscope>(HoroscopeErrors.InvalidLuckyNumbers);
        }

        if (luckyColors == null || luckyColors.Count == 0)
        {
            return Result.Ko<Horoscope>(HoroscopeErrors.InvalidLuckyColors);
        }

        if (keywords == null || keywords.Count == 0)
        {
            return Result.Ko<Horoscope>(HoroscopeErrors.InvalidKeywords);
        }

        var horoscope = new Horoscope(
            zodiacSignId,
            period,
            date,
            generalPrediction,
            lovePrediction,
            careerPrediction,
            healthPrediction,
            luckyNumbers,
            luckyColors,
            moodScore,
            keywords);

        return Result.Ok(horoscope);
    }
}
