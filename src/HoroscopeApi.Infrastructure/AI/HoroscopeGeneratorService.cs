using Hermes.Results;
using HoroscopeApi.Application.Infrastructure.AI;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;
using HoroscopeApi.Domain.AI;

namespace HoroscopeApi.Infrastructure.AI;

public sealed class HoroscopeGeneratorService : IHoroscopeGeneratorService
{
    private readonly IHuggingFaceClient _huggingFaceClient;
    private readonly IZodiacSignRepository _zodiacSignRepository;

    public HoroscopeGeneratorService(
        IHuggingFaceClient huggingFaceClient,
        IZodiacSignRepository zodiacSignRepository)
    {
        _huggingFaceClient = huggingFaceClient;
        _zodiacSignRepository = zodiacSignRepository;
    }

    public async Task<Result<Horoscope>> GenerateDailyHoroscopeAsync(
        ZodiacSign sign,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        // Fetch zodiac sign info from database
        var zodiacSignQuery = new GetZodiacSignBySignRepositoryQuery(sign);
        var zodiacSignInfo = await _zodiacSignRepository.GetBySignAsync(zodiacSignQuery, cancellationToken);
        
        if (zodiacSignInfo == null)
        {
            return Result.Ko<Horoscope>(ZodiacSignErrors.NotFound(sign));
        }

        // Generate horoscope for the specific sign
        var request = new GenerateHoroscopeRequest(date, zodiacSignInfo);
        var aiResult = await _huggingFaceClient.GenerateHoroscopeAsync(request, cancellationToken);
        
        if (!aiResult.IsSuccess)
        {
            return Result.Ko<Horoscope>(aiResult.Errors);
        }

        var horoscopeData = aiResult.Value!;

        // Validate the sign matches
        if (!horoscopeData.Sign.Equals(sign.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return Result.Ko<Horoscope>(AiErrors.InvalidResponse);
        }

        // Generate lucky numbers deterministically
        var luckyNumbers = LuckyNumberGenerator.Generate(sign, date).ToList();

        // Create the Horoscope entity
        var horoscopeResult = Horoscope.Create(
            zodiacSignId: zodiacSignInfo.Id,
            period: HoroscopePeriod.Daily,
            date: date,
            generalPrediction: horoscopeData.General,
            lovePrediction: horoscopeData.Love,
            careerPrediction: horoscopeData.Career,
            healthPrediction: horoscopeData.Health,
            luckyNumbers: luckyNumbers,
            luckyColors: horoscopeData.LuckyColors,
            moodScore: horoscopeData.MoodScore,
            keywords: horoscopeData.Keywords);

        if (!horoscopeResult.IsSuccess)
        {
            return Result.Ko<Horoscope>(horoscopeResult.Errors);
        }

        return Result.Ok(horoscopeResult.Value!);
    }
}
