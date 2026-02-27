using HoroscopeApi.Application.Features.Shared;
using HoroscopeApi.Application.Infrastructure.AI;
using HoroscopeApi.Application.Infrastructure.Caching;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetHoroscope;

internal sealed class GetHoroscopeQueryHandler(
    IHoroscopeRepository horoscopeRepository,
    IHoroscopeGeneratorService horoscopeGeneratorService,
    IUnitOfWork unitOfWork,
    IRedisCache cache)
    : IQueryHandler<GetHoroscopeQuery, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetHoroscopeQuery query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ZodiacSign>(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        // Determine the period and date
        // If date is provided, it takes precedence and we use daily period
        // Otherwise, use the period parameter (default to daily if not specified)
        HoroscopePeriod period;
        DateOnly date;

        if (query.Date.HasValue)
        {
            // Date provided - use it and default to daily period
            date = query.Date.Value;
            period = HoroscopePeriod.Daily;
        }
        else
        {
            // No date provided - use period (default to daily)
            period = query.Period ?? HoroscopePeriod.Daily;
            date = period == HoroscopePeriod.Yearly
                ? new DateOnly(DateTime.UtcNow.Year, 1, 1)
                : DateOnly.FromDateTime(DateTime.UtcNow);
        }

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
            Result<Horoscope> generationResult = await GenerateHoroscopeByPeriod(
                zodiacSign,
                period,
                date,
                cancellationToken);

            if (!generationResult.IsSuccess)
            {
                return Result.Ko<Response<HoroscopeData>>(generationResult.Errors);
            }

            horoscope = generationResult.Value!;

            // Save the generated horoscope to the database
            await horoscopeRepository.AddAsync(horoscope, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload to populate ZodiacSignInfo navigation property
            horoscope = await horoscopeRepository.GetBySignAndPeriodAsync(
                repositoryQuery,
                cancellationToken);

            if (horoscope is null)
            {
                return Result.Ko<Response<HoroscopeData>>(HoroscopeErrors.NotFound);
            }
        }

        HoroscopeData data = HoroscopeData.ToHoroscopeData(horoscope);

        // Cache the result
        await cache.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

        Response<HoroscopeData> response = Response<HoroscopeData>.Create(data);
        return Result.Ok(response);
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