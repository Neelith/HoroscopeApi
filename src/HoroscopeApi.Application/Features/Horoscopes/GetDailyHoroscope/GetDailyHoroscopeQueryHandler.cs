using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.HoroscopeService;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;

internal sealed class GetDailyHoroscopeQueryHandler(
    IHoroscopeQueryService horoscopeQueryService,
    IDateTimeProvider dateTimeProvider)
    : IQueryHandler<GetDailyHoroscopeQuery, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetDailyHoroscopeQuery query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        DateOnly date = DateOnly.FromDateTime(dateTimeProvider.UtcNow);

        Result<(HoroscopeData Data, bool IsCached)> serviceResult =
            await horoscopeQueryService.GetOrGenerateHoroscopeAsync(
                zodiacSign, HoroscopePeriod.Daily, date, cancellationToken);

        if (serviceResult.IsFailure)
        {
            return Result.Ko<Response<HoroscopeData>>(serviceResult.Errors);
        }

        (HoroscopeData data, bool isCached) = serviceResult.Value;
        Dictionary<string, string?> attributes = new() { { "cached", isCached ? "true" : "false" } };
        return Result.Ok(Response<HoroscopeData>.Create(data, attributes));
    }
}