using HoroscopeApi.Application.Features.Horoscopes.Shared;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDateHoroscope;

internal sealed class GetDateHoroscopeHandler(HoroscopeQueryService horoscopeQueryService)
    : IQueryHandler<GetDateHoroscopeRequest, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetDateHoroscopeRequest query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        return await horoscopeQueryService.GetOrGenerateHoroscopeAsync(
            zodiacSign, HoroscopePeriod.Daily, query.Date, cancellationToken);
    }
}
