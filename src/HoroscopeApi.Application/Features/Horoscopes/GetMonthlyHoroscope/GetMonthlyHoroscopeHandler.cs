using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.HoroscopeService;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetMonthlyHoroscope;

internal sealed class GetMonthlyHoroscopeHandler(
    IHoroscopeQueryService horoscopeQueryService,
    IDateTimeProvider dateTimeProvider)
    : IQueryHandler<GetMonthlyHoroscopeQuery, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetMonthlyHoroscopeQuery query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        DateOnly date = DateOnly.FromDateTime(dateTimeProvider.UtcNow);

        return await horoscopeQueryService.GetOrGenerateHoroscopeAsync(
            zodiacSign, HoroscopePeriod.Monthly, date, cancellationToken);
    }
}