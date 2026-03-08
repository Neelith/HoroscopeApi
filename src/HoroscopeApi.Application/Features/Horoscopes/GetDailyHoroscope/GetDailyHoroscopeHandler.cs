using HoroscopeApi.Application.Features.Horoscopes.Shared;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;

internal sealed class GetDailyHoroscopeHandler(HoroscopeQueryService horoscopeQueryService)
    : IQueryHandler<GetDailyHoroscopeRequest, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetDailyHoroscopeRequest query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        DateOnly date = DateOnly.FromDateTime(horoscopeQueryService.DateTimeProvider.UtcNow);

        return await horoscopeQueryService.GetOrGenerateHoroscopeAsync(
            zodiacSign, HoroscopePeriod.Daily, date, cancellationToken);
    }
}
