using HoroscopeApi.Domain.ZodiacSigns;
using HoroscopeApi.Shared.Domain;
using Hermes.Results;

namespace HoroscopeApi.Domain.Horoscopes;

public sealed class Horoscope : AuditableEntity
{
    public int Id { get; private set; }
    public ZodiacSign Sign { get; private set; }
    public HoroscopePeriod Period { get; private set; }
    public DateOnly Date { get; private set; }
    public string GeneralPrediction { get; private set; } = string.Empty;
    public string? LovePrediction { get; private set; }
    public string? CareerPrediction { get; private set; }
    public string? HealthPrediction { get; private set; }
    public string LuckyNumbers { get; private set; } = string.Empty;
    public string LuckyColors { get; private set; } = string.Empty;
    public int MoodScore { get; private set; }
    public string Keywords { get; private set; } = string.Empty;

    //EF constructor - required for entity materialization
    private Horoscope() { }

    private Horoscope(
        ZodiacSign sign,
        HoroscopePeriod period,
        DateOnly date,
        string generalPrediction,
        string? lovePrediction,
        string? careerPrediction,
        string? healthPrediction,
        string luckyNumbers,
        string luckyColors,
        int moodScore,
        string keywords)
    {
        Sign = sign;
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
        ZodiacSign sign,
        HoroscopePeriod period,
        DateOnly date,
        string generalPrediction,
        string? lovePrediction,
        string? careerPrediction,
        string? healthPrediction,
        string luckyNumbers,
        string luckyColors,
        int moodScore,
        string keywords)
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

        if (string.IsNullOrWhiteSpace(luckyNumbers))
        {
            return Result.Ko<Horoscope>(HoroscopeErrors.InvalidLuckyNumbers);
        }

        if (string.IsNullOrWhiteSpace(luckyColors))
        {
            return Result.Ko<Horoscope>(HoroscopeErrors.InvalidLuckyColors);
        }

        if (string.IsNullOrWhiteSpace(keywords))
        {
            return Result.Ko<Horoscope>(HoroscopeErrors.InvalidKeywords);
        }

        var horoscope = new Horoscope(
            sign,
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
