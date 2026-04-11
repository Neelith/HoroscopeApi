using HoroscopeApi.Application.Infrastructure.Caching;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Application.Services.HoroscopeService;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using HoroscopeApi.Domain.ZodiacSigns;
using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Infrastructure.HoroscopeService;

internal sealed class HoroscopeQueryService(
    ILogger<HoroscopeQueryService> logger,
    IHoroscopeRepository horoscopeRepository,
    IHoroscopeGeneratorService horoscopeGeneratorService,
    IUnitOfWork unitOfWork,
    IRedisCache cache) : IHoroscopeQueryService
{
    public async Task<Result<(HoroscopeData Data, bool IsCached)>> GetOrGenerateHoroscopeAsync(
        ZodiacSign zodiacSign,
        HoroscopePeriod period,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        string cacheKey = $"horoscope:{(int)zodiacSign}:{(int)period}:{date:yyyy-MM-dd}";
        string lockKey = $"horoscope:lock:{(int)zodiacSign}:{(int)period}:{date:yyyy-MM-dd}";

        HoroscopeData? cachedData = await cache.GetAsync<HoroscopeData>(cacheKey, cancellationToken);
        if (cachedData is not null)
        {
            return Result.Ok((cachedData, true));
        }

        string? lockValue = await cache.TryAcquireLockAsync(lockKey, TimeSpan.FromSeconds(30), cancellationToken);

        if (lockValue is null)
        {
            await Task.Delay(500, cancellationToken);
            cachedData = await cache.GetAsync<HoroscopeData>(cacheKey, cancellationToken);

            if (cachedData is not null)
                return Result.Ok((cachedData, true));

            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1000, cancellationToken);
                cachedData = await cache.GetAsync<HoroscopeData>(cacheKey, cancellationToken);
                if (cachedData is not null)
                    return Result.Ok((cachedData, true));
            }

            return Result.Ko<(HoroscopeData, bool)>(HoroscopeErrors.NotFound);
        }

        try
        {
            return await GenerateHoroscopeInternalAsync(zodiacSign, period, date, cacheKey, cancellationToken);
        }
        finally
        {
            await cache.ReleaseLockAsync(lockKey, lockValue, cancellationToken);
        }
    }

    private async Task<Result<(HoroscopeData Data, bool IsCached)>> GenerateHoroscopeInternalAsync(
        ZodiacSign zodiacSign,
        HoroscopePeriod period,
        DateOnly date,
        string cacheKey,
        CancellationToken cancellationToken)
    {
        GetHoroscopeBySignAndPeriodRepositoryQuery repositoryQuery = new(zodiacSign, period, date);

        Horoscope? horoscope = await horoscopeRepository.GetBySignAndPeriodAsync(repositoryQuery, cancellationToken);

        if (horoscope is null)
        {
            Result<Horoscope?> generateResult =
                await GenerateHoroscopeUsingAiAsync(zodiacSign, period, date, repositoryQuery, cancellationToken);

            if (generateResult.IsFailure)
            {
                logger.LogError(
                    "Failed to generate horoscope for sign {Sign} and period {Period}: {@Errors}",
                    zodiacSign, period, generateResult.Errors);
                return Result.Ko<(HoroscopeData, bool)>(HoroscopeErrors.NotFound);
            }

            horoscope = generateResult.Value!;
        }

        HoroscopeData data = HoroscopeData.ToHoroscopeData(horoscope);

        TimeSpan cacheTtl = date >= DateOnly.FromDateTime(DateTime.UtcNow)
            ? TimeSpan.FromHours(24)
            : TimeSpan.FromDays(365);
        await cache.SetAsync(cacheKey, data, cacheTtl, cancellationToken);

        return Result.Ok((data, false));
    }

    private async Task<Result<Horoscope?>> GenerateHoroscopeUsingAiAsync(
        ZodiacSign zodiacSign,
        HoroscopePeriod period,
        DateOnly date,
        GetHoroscopeBySignAndPeriodRepositoryQuery repositoryQuery,
        CancellationToken cancellationToken)
    {
        Result<Horoscope> generationResult = await GenerateHoroscopeByPeriod(
            zodiacSign, period, date, cancellationToken);

        if (!generationResult.IsSuccess)
        {
            return Result.Ko<Horoscope?>(generationResult.Errors);
        }

        Horoscope horoscope = generationResult.Value!;

        await horoscopeRepository.AddAsync(horoscope, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload to populate ZodiacSignInfo navigation property
        horoscope = (await horoscopeRepository.GetBySignAndPeriodAsync(repositoryQuery, cancellationToken))!;

        return horoscope;
    }

    private async Task<Result<Horoscope>> GenerateHoroscopeByPeriod(
        ZodiacSign sign,
        HoroscopePeriod period,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        return period switch
        {
            HoroscopePeriod.Daily => await horoscopeGeneratorService.GenerateDailyHoroscopeAsync(sign, date,
                cancellationToken),
            HoroscopePeriod.Weekly => await horoscopeGeneratorService.GenerateWeeklyHoroscopeAsync(sign, date,
                cancellationToken),
            HoroscopePeriod.Monthly => await horoscopeGeneratorService.GenerateMonthlyHoroscopeAsync(sign, date,
                cancellationToken),
            HoroscopePeriod.Yearly => await horoscopeGeneratorService.GenerateYearlyHoroscopeAsync(sign, date.Year,
                cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(period), period, "Invalid horoscope period")
        };
    }
}