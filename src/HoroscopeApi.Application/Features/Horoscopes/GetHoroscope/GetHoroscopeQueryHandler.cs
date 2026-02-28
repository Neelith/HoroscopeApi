using HoroscopeApi.Application.Infrastructure.Caching;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using HoroscopeApi.Domain.ZodiacSigns;
using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Application.Features.Horoscopes.GetHoroscope;

internal sealed class GetHoroscopeQueryHandler(
    ILogger<GetHoroscopeQueryHandler> logger,
    IHoroscopeRepository horoscopeRepository,
    IHoroscopeGeneratorService horoscopeGeneratorService,
    IUnitOfWork unitOfWork,
    IRedisCache cache,
    IDateTimeProvider dateTimeProvider)
    : IQueryHandler<GetHoroscopeQuery, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetHoroscopeQuery query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        // Determine the period and date
        // If date is provided, it takes precedence and we use daily period
        // Otherwise, use the period parameter (default to daily if not specified)
        HoroscopePeriod period = query.Period ?? HoroscopePeriod.Daily;

        DateTime utcNow = dateTimeProvider.UtcNow;

        // Date provided - use it and default to daily period
        DateOnly date = query.Date ??
                        (period == HoroscopePeriod.Yearly
                            ? new DateOnly(utcNow.Year, 1, 1)
                            : DateOnly.FromDateTime(utcNow));

        // Try cache first
        string cacheKey = $"horoscope:{zodiacSign}:{period}:{date:yyyy-MM-dd}";
        HoroscopeData? cachedData = await cache.GetAsync<HoroscopeData>(cacheKey, cancellationToken);
        if (cachedData is not null)
        {
            return Result.Ok(Response<HoroscopeData>.Create(cachedData));
        }

        GetHoroscopeBySignAndPeriodRepositoryQuery repositoryQuery = new(
            zodiacSign,
            period,
            date);

        Horoscope? horoscope = await horoscopeRepository.GetBySignAndPeriodAsync(
            repositoryQuery,
            cancellationToken);

        if (horoscope is null)
        {
            // Generate horoscope for the requested zodiac sign using AI
            Result<Horoscope?> generateHoroscopeUsingAiResult =
                await GenerateHoroscopeUsingAiAsync(zodiacSign, period, date, repositoryQuery, cancellationToken);

            if (generateHoroscopeUsingAiResult.IsFailure)
            {
                logger.LogError("Failed to generate horoscope for sign {Sign} and period {Period}: {@Errors}",
                    zodiacSign, period, generateHoroscopeUsingAiResult.Errors);
                return Result.Ko<Response<HoroscopeData>>(HoroscopeErrors.NotFound);
            }

            horoscope = generateHoroscopeUsingAiResult.Value!;
        }

        HoroscopeData data = HoroscopeData.ToHoroscopeData(horoscope);

        // Cache the result
        await cache.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

        Response<HoroscopeData> response = Response<HoroscopeData>.Create(data);

        return Result.Ok(response);
    }

    private async Task<Result<Horoscope?>> GenerateHoroscopeUsingAiAsync(
        ZodiacSign zodiacSign,
        HoroscopePeriod period,
        DateOnly date,
        GetHoroscopeBySignAndPeriodRepositoryQuery repositoryQuery,
        CancellationToken cancellationToken)
    {
        Result<Horoscope> generationResult = await GenerateHoroscopeByPeriod(
            zodiacSign,
            period,
            date,
            cancellationToken);

        if (!generationResult.IsSuccess)
        {
            return Result.Ko<Horoscope?>(generationResult.Errors);
        }

        Horoscope? horoscope = generationResult.Value!;

        // Save the generated horoscope to the database
        await horoscopeRepository.AddAsync(horoscope, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload to populate ZodiacSignInfo navigation property
        horoscope = await horoscopeRepository.GetBySignAndPeriodAsync(
            repositoryQuery,
            cancellationToken);

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