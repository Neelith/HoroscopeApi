using System.Collections.Generic;
using Hermes.Handlers;
using Hermes.Responses;
using HoroscopeApi.Application.Features.Shared;
using HoroscopeApi.Application.Infrastructure.AI;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using HoroscopeApi.Domain.ZodiacSigns;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;

internal sealed class GetDailyHoroscopeQueryHandler(
    IHoroscopeRepository horoscopeRepository,
    IZodiacSignRepository zodiacSignRepository,
    IHoroscopeGeneratorService horoscopeGeneratorService,
    IUnitOfWork unitOfWork)
    : IQueryHandler<GetDailyHoroscopeQuery, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetDailyHoroscopeQuery query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ZodiacSign>(query.SignName, true, out var zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        var zodiacSignQuery = new GetZodiacSignBySignRepositoryQuery(zodiacSign);
        var signInfo = await zodiacSignRepository.GetBySignAsync(zodiacSignQuery, cancellationToken);

        if (signInfo is null)
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.NotFound(zodiacSign));
        }

        var date = query.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var repositoryQuery = new GetHoroscopeBySignAndPeriodRepositoryQuery(
            signInfo.Id,
            HoroscopePeriod.Daily,
            date);
        
        var horoscope = await horoscopeRepository.GetBySignAndPeriodAsync(
            repositoryQuery,
            cancellationToken);

        if (horoscope is null)
        {
            // Generate horoscope for the requested zodiac sign using AI
            var generationResult = await horoscopeGeneratorService.GenerateDailyHoroscopeAsync(
                zodiacSign,
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

        var data = MapToData(horoscope!, horoscope.ZodiacSignInfo);
        var response = Response<HoroscopeData>.Create(data);
        return Result.Ok(response);
    }

    private static HoroscopeData MapToData(Horoscope horoscope, ZodiacSignInfo signInfo)
    {
        return new HoroscopeData
        {
            Sign = horoscope.ZodiacSignInfo.Sign.ToString().ToLowerInvariant(),
            SignInfo = new ZodiacSignInfoData
            {
                Name = signInfo.Name,
                Symbol = signInfo.Symbol,
                Element = signInfo.Element.ToString(),
                Quality = signInfo.Quality.ToString(),
                Polarity = signInfo.Polarity.ToString(),
                RulingPlanet = signInfo.RulingPlanet,
                DateRange = GetDateRangeString(signInfo.StartMonth, signInfo.StartDay, signInfo.EndMonth, signInfo.EndDay),
                Description = signInfo.Description
            },
            Period = horoscope.Period.ToString().ToLowerInvariant(),
            Date = horoscope.Date,
            Predictions = new HoroscopePredictions
            {
                General = horoscope.GeneralPrediction,
                Love = horoscope.LovePrediction,
                Career = horoscope.CareerPrediction,
                Health = horoscope.HealthPrediction
            },
            LuckyNumbers = horoscope.LuckyNumbers,
            LuckyColors = horoscope.LuckyColors,
            MoodScore = horoscope.MoodScore,
            Keywords = horoscope.Keywords
        };
    }

    private static string GetDateRangeString(int startMonth, int startDay, int endMonth, int endDay)
    {
        var startMonthName = new DateOnly(2000, startMonth, 1).ToString("MMMM");
        var endMonthName = new DateOnly(2000, endMonth, 1).ToString("MMMM");
        return $"{startMonthName} {startDay} - {endMonthName} {endDay}";
    }
}
