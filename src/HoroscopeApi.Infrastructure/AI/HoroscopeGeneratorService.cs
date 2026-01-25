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

    public async Task<Result<List<Horoscope>>> GenerateDailyHoroscopesAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        // Fetch all zodiac sign descriptions from database
        var allZodiacSigns = await _zodiacSignRepository.GetAllAsync(cancellationToken);
        
        if (allZodiacSigns.Count != 12)
        {
            return Result.Ko<List<Horoscope>>(AIErrors.GenerationFailed);
        }

        // Generate batch horoscopes for all 12 signs
        var request = new GenerateBatchHoroscopesRequest(date, allZodiacSigns);
        var batchResult = await _huggingFaceClient.GenerateBatchHoroscopesAsync(request, cancellationToken);
        
        if (!batchResult.IsSuccess)
        {
            return Result.Ko<List<Horoscope>>(batchResult.Errors);
        }

        // Validate we got all 12 horoscopes (all-or-nothing approach)
        if (batchResult.Value!.Horoscopes.Count != 12)
        {
            return Result.Ko<List<Horoscope>>(AIErrors.InvalidResponse);
        }

        var horoscopes = new List<Horoscope>();

        // Convert each AI horoscope to domain entity
        foreach (var horoscopeData in batchResult.Value.Horoscopes)
        {
            // Parse the zodiac sign from the response
            if (!Enum.TryParse<ZodiacSign>(horoscopeData.Sign, true, out var sign))
            {
                return Result.Ko<List<Horoscope>>(AIErrors.InvalidResponse);
            }

            // Get the zodiac sign info for this sign
            var zodiacSignInfo = allZodiacSigns.FirstOrDefault(z => z.Sign == sign);
            if (zodiacSignInfo == null)
            {
                return Result.Ko<List<Horoscope>>(AIErrors.GenerationFailed);
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
                return Result.Ko<List<Horoscope>>(horoscopeResult.Errors);
            }

            horoscopes.Add(horoscopeResult.Value!);
        }

        return Result.Ok(horoscopes);
    }
}
