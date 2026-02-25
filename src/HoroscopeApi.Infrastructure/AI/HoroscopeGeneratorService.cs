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
    private readonly IHoroscopePromptBuilder _promptBuilder;

    public HoroscopeGeneratorService(
        IHuggingFaceClient huggingFaceClient,
        IZodiacSignRepository zodiacSignRepository,
        IHoroscopePromptBuilder promptBuilder)
    {
        _huggingFaceClient = huggingFaceClient;
        _zodiacSignRepository = zodiacSignRepository;
        _promptBuilder = promptBuilder;
    }

    public async Task<Result<Horoscope>> GenerateDailyHoroscopeAsync(
        ZodiacSign sign,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await GenerateHoroscopeAsync(
            sign,
            date,
            HoroscopePeriod.Daily,
            isYearly: false,
            year: null,
            cancellationToken);
    }

    public async Task<Result<Horoscope>> GenerateWeeklyHoroscopeAsync(
        ZodiacSign sign,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await GenerateHoroscopeAsync(
            sign,
            date,
            HoroscopePeriod.Weekly,
            isYearly: false,
            year: null,
            cancellationToken);
    }

    public async Task<Result<Horoscope>> GenerateMonthlyHoroscopeAsync(
        ZodiacSign sign,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await GenerateHoroscopeAsync(
            sign,
            date,
            HoroscopePeriod.Monthly,
            isYearly: false,
            year: null,
            cancellationToken);
    }

    public async Task<Result<Horoscope>> GenerateYearlyHoroscopeAsync(
        ZodiacSign sign,
        int year,
        CancellationToken cancellationToken = default)
    {
        var date = new DateOnly(year, 1, 1);
        return await GenerateHoroscopeAsync(
            sign,
            date,
            HoroscopePeriod.Yearly,
            isYearly: true,
            year: year,
            cancellationToken);
    }

    private async Task<Result<Horoscope>> GenerateHoroscopeAsync(
        ZodiacSign sign,
        DateOnly date,
        HoroscopePeriod period,
        bool isYearly,
        int? year,
        CancellationToken cancellationToken)
    {
        // Fetch zodiac sign info from database
        var zodiacSignQuery = new GetZodiacSignBySignRepositoryQuery(sign);
        var zodiacSignInfo = await _zodiacSignRepository.GetBySignAsync(zodiacSignQuery, cancellationToken);
        
        if (zodiacSignInfo == null)
        {
            return Result.Ko<Horoscope>(ZodiacSignErrors.NotFound(sign));
        }

        // Generate horoscope using AI with appropriate prompt
        Result<HuggingFaceHoroscopeData> aiResult;
        if (isYearly)
        {
            aiResult = await GenerateWithYearlyPrompt(year!.Value, zodiacSignInfo, cancellationToken);
        }
        else if (period == HoroscopePeriod.Weekly)
        {
            aiResult = await GenerateWithWeeklyPrompt(date, zodiacSignInfo, cancellationToken);
        }
        else if (period == HoroscopePeriod.Monthly)
        {
            aiResult = await GenerateWithMonthlyPrompt(date, zodiacSignInfo, cancellationToken);
        }
        else
        {
            var request = new GenerateHoroscopeRequest(date, zodiacSignInfo);
            aiResult = await _huggingFaceClient.GenerateHoroscopeAsync(request, cancellationToken);
        }
        
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
            period: period,
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

    private async Task<Result<HuggingFaceHoroscopeData>> GenerateWithYearlyPrompt(
        int year,
        ZodiacSignInfo zodiacSignInfo,
        CancellationToken cancellationToken)
    {
        var messages = _promptBuilder.BuildYearlyMessage(year, zodiacSignInfo);
        return await _huggingFaceClient.GenerateWithMessages(messages, cancellationToken);
    }

    private async Task<Result<HuggingFaceHoroscopeData>> GenerateWithWeeklyPrompt(
        DateOnly date,
        ZodiacSignInfo zodiacSignInfo,
        CancellationToken cancellationToken)
    {
        var messages = _promptBuilder.BuildWeeklyMessage(date, zodiacSignInfo);
        return await _huggingFaceClient.GenerateWithMessages(messages, cancellationToken);
    }

    private async Task<Result<HuggingFaceHoroscopeData>> GenerateWithMonthlyPrompt(
        DateOnly date,
        ZodiacSignInfo zodiacSignInfo,
        CancellationToken cancellationToken)
    {
        var messages = _promptBuilder.BuildMonthlyMessage(date, zodiacSignInfo);
        return await _huggingFaceClient.GenerateWithMessages(messages, cancellationToken);
    }
}
