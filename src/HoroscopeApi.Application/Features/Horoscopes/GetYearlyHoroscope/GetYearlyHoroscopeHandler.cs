using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.HoroscopeService;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetYearlyHoroscope;

internal sealed class GetYearlyHoroscopeHandler(
    IHoroscopeQueryService horoscopeQueryService,
    IDateTimeProvider dateTimeProvider)
    : IQueryHandler<GetYearlyHoroscopeQuery, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetYearlyHoroscopeQuery query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        int year = query.Year ?? dateTimeProvider.UtcNow.Year;
        DateOnly date = new(year, 1, 1);

        return await horoscopeQueryService.GetOrGenerateHoroscopeAsync(
            zodiacSign, HoroscopePeriod.Yearly, date, cancellationToken);
    }
}