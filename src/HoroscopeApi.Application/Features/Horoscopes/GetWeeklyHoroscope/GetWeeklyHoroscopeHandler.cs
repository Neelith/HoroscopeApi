using HoroscopeApi.Application.Features.Horoscopes.Shared;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetWeeklyHoroscope;

internal sealed class GetWeeklyHoroscopeHandler(HoroscopeQueryService horoscopeQueryService)
    : IQueryHandler<GetWeeklyHoroscopeRequest, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetWeeklyHoroscopeRequest query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        DateOnly date = DateOnly.FromDateTime(horoscopeQueryService.DateTimeProvider.UtcNow);

        return await horoscopeQueryService.GetOrGenerateHoroscopeAsync(
            zodiacSign, HoroscopePeriod.Weekly, date, cancellationToken);
    }
}
