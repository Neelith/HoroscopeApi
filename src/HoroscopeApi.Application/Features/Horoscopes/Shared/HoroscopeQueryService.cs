using HoroscopeApi.Application.Infrastructure.Caching;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using HoroscopeApi.Domain.ZodiacSigns;
using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Application.Features.Horoscopes.Shared;

internal sealed class HoroscopeQueryService(
    ILogger<HoroscopeQueryService> logger,
    IHoroscopeRepository horoscopeRepository,
    IHoroscopeGeneratorService horoscopeGeneratorService,
    IUnitOfWork unitOfWork,
    IRedisCache cache,
    IDateTimeProvider dateTimeProvider)
{
    public IDateTimeProvider DateTimeProvider => dateTimeProvider;

    public async Task<Result<Response<HoroscopeData>>> GetOrGenerateHoroscopeAsync(
        ZodiacSign zodiacSign,
        HoroscopePeriod period,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        // Try cache first
        string cacheKey = $"horoscope:{zodiacSign}:{period}:{date:yyyy-MM-dd}";
        HoroscopeData? cachedData = await cache.GetAsync<HoroscopeData>(cacheKey, cancellationToken);
        if (cachedData is not null)
        {
            return Result.Ok(Response<HoroscopeData>.Create(cachedData));
        }

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
                return Result.Ko<Response<HoroscopeData>>(HoroscopeErrors.NotFound);
            }

            horoscope = generateResult.Value!;
        }

        HoroscopeData data = HoroscopeData.ToHoroscopeData(horoscope);

        await cache.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

        return Result.Ok(Response<HoroscopeData>.Create(data));
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
