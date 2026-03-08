using HoroscopeApi.Application.Features.Horoscopes.Shared;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetYearlyHoroscope;

internal sealed class GetYearlyHoroscopeHandler(HoroscopeQueryService horoscopeQueryService)
    : IQueryHandler<GetYearlyHoroscopeRequest, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetYearlyHoroscopeRequest query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        int year = query.Year ?? horoscopeQueryService.DateTimeProvider.UtcNow.Year;
        DateOnly date = new(year, 1, 1);

        return await horoscopeQueryService.GetOrGenerateHoroscopeAsync(
            zodiacSign, HoroscopePeriod.Yearly, date, cancellationToken);
    }
}
