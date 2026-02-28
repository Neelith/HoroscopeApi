using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Domain.Constants;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;

namespace HoroscopeApi.Infrastructure.AI;

public sealed class HoroscopeGeneratorService : IHoroscopeGeneratorService
{
    private readonly IHuggingFaceClient _huggingFaceClient;
    private readonly IHoroscopePromptBuilder _promptBuilder;
    private readonly IZodiacSignRepository _zodiacSignRepository;

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
            false,
            null,
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
            false,
            null,
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
            false,
            null,
            cancellationToken);
    }

    public async Task<Result<Horoscope>> GenerateYearlyHoroscopeAsync(
        ZodiacSign sign,
        int year,
        CancellationToken cancellationToken = default)
    {
        DateOnly date = new(year, 1, 1);
        return await GenerateHoroscopeAsync(
            sign,
            date,
            HoroscopePeriod.Yearly,
            true,
            year,
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
        GetZodiacSignBySignRepositoryQuery zodiacSignQuery = new(sign);
        ZodiacSignInfo? zodiacSignInfo = await _zodiacSignRepository.GetBySignAsync(zodiacSignQuery, cancellationToken);

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
            GenerateHoroscopeRequest request = new(date, zodiacSignInfo);
            aiResult = await _huggingFaceClient.GenerateHoroscopeAsync(request, cancellationToken);
        }

        if (!aiResult.IsSuccess)
        {
            return Result.Ko<Horoscope>(aiResult.Errors);
        }

        HuggingFaceHoroscopeData horoscopeData = aiResult.Value!;

        // Validate the sign matches
        if (!horoscopeData.Sign.Equals(sign.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return Result.Ko<Horoscope>(AiErrors.InvalidResponse);
        }

        // Generate lucky numbers deterministically
        List<int> luckyNumbers = LuckyNumberGenerator.Generate(sign, date).ToList();

        // Create the Horoscope entity
        Result<Horoscope> horoscopeResult = Horoscope.Create(
            zodiacSignInfo.Id,
            period,
            date,
            horoscopeData.General,
            horoscopeData.Love,
            horoscopeData.Career,
            horoscopeData.Health,
            luckyNumbers,
            horoscopeData.LuckyColors,
            horoscopeData.MoodScore,
            horoscopeData.Keywords);

        return horoscopeResult.IsSuccess
            ? Result.Ok(horoscopeResult.Value!)
            : Result.Ko<Horoscope>(horoscopeResult.Errors);
    }

    private async Task<Result<HuggingFaceHoroscopeData>> GenerateWithYearlyPrompt(
        int year,
        ZodiacSignInfo zodiacSignInfo,
        CancellationToken cancellationToken)
    {
        List<ChatMessage> messages = _promptBuilder.BuildYearlyMessage(year, zodiacSignInfo);
        return await _huggingFaceClient.GenerateWithMessages(messages, cancellationToken);
    }

    private async Task<Result<HuggingFaceHoroscopeData>> GenerateWithWeeklyPrompt(
        DateOnly date,
        ZodiacSignInfo zodiacSignInfo,
        CancellationToken cancellationToken)
    {
        List<ChatMessage> messages = _promptBuilder.BuildWeeklyMessage(date, zodiacSignInfo);
        return await _huggingFaceClient.GenerateWithMessages(messages, cancellationToken);
    }

    private async Task<Result<HuggingFaceHoroscopeData>> GenerateWithMonthlyPrompt(
        DateOnly date,
        ZodiacSignInfo zodiacSignInfo,
        CancellationToken cancellationToken)
    {
        List<ChatMessage> messages = _promptBuilder.BuildMonthlyMessage(date, zodiacSignInfo);
        return await _huggingFaceClient.GenerateWithMessages(messages, cancellationToken);
    }
}