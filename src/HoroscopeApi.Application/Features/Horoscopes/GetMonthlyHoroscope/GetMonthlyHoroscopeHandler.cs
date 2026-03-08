using HoroscopeApi.Application.Features.Horoscopes.Shared;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetMonthlyHoroscope;

internal sealed class GetMonthlyHoroscopeHandler(HoroscopeQueryService horoscopeQueryService)
    : IQueryHandler<GetMonthlyHoroscopeRequest, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetMonthlyHoroscopeRequest query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        DateOnly date = DateOnly.FromDateTime(horoscopeQueryService.DateTimeProvider.UtcNow);

        return await horoscopeQueryService.GetOrGenerateHoroscopeAsync(
            zodiacSign, HoroscopePeriod.Monthly, date, cancellationToken);
    }
}
