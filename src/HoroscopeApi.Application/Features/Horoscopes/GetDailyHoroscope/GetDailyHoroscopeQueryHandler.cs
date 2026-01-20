using System.Collections.Generic;
using Hermes.Handlers;
using HoroscopeApi.Application.Features.Shared;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using HoroscopeApi.Domain.ZodiacSigns;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;

internal sealed class GetDailyHoroscopeQueryHandler(
    IHoroscopeRepository horoscopeRepository,
    IZodiacSignRepository zodiacSignRepository)
    : IQueryHandler<GetDailyHoroscopeQuery, HoroscopeResponse>
{
    public async Task<Result<HoroscopeResponse>> Handle(
        GetDailyHoroscopeQuery query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ZodiacSign>(query.SignName, true, out var zodiacSign))
        {
            return Result.Ko<HoroscopeResponse>(ZodiacSignErrors.InvalidName);
        }

        var zodiacSignQuery = new GetZodiacSignBySignRepositoryQuery(zodiacSign);
        var signInfo = await zodiacSignRepository.GetBySignAsync(zodiacSignQuery, cancellationToken);

        if (signInfo is null)
        {
            return Result.Ko<HoroscopeResponse>(ZodiacSignErrors.NotFound(zodiacSign));
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
            return Result.Ko<HoroscopeResponse>(HoroscopeErrors.NotFound);
        }

        var response = MapToResponse(horoscope, horoscope.ZodiacSignInfo);
        return Result.Ok(response);
    }

    private static HoroscopeResponse MapToResponse(Horoscope horoscope, ZodiacSignInfo signInfo)
    {
        return new HoroscopeResponse
        {
            Sign = horoscope.ZodiacSignInfo.Sign.ToString().ToLowerInvariant(),
            SignInfo = new ZodiacSignInfoResponse
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
            Predictions = new HoroscopePredictionsResponse
            {
                General = horoscope.GeneralPrediction,
                Love = horoscope.LovePrediction,
                Career = horoscope.CareerPrediction,
                Health = horoscope.HealthPrediction
            },
            LuckyNumbers = JsonSerializer.Deserialize<List<int>>(horoscope.LuckyNumbers) ?? [],
            LuckyColors = JsonSerializer.Deserialize<List<string>>(horoscope.LuckyColors) ?? [],
            MoodScore = horoscope.MoodScore,
            Keywords = JsonSerializer.Deserialize<List<string>>(horoscope.Keywords) ?? []
        };
    }

    private static string GetDateRangeString(int startMonth, int startDay, int endMonth, int endDay)
    {
        var startMonthName = new DateOnly(2000, startMonth, 1).ToString("MMMM");
        var endMonthName = new DateOnly(2000, endMonth, 1).ToString("MMMM");
        return $"{startMonthName} {startDay} - {endMonthName} {endDay}";
    }
}
