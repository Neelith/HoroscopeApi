using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.HoroscopeService;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDateHoroscope;

internal sealed class GetDateHoroscopeQueryHandler(IHoroscopeQueryService horoscopeQueryService)
    : IQueryHandler<GetDateHoroscopeQuery, Response<HoroscopeData>>
{
    public async Task<Result<Response<HoroscopeData>>> Handle(
        GetDateHoroscopeQuery query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(query.SignName, true, out ZodiacSign zodiacSign))
        {
            return Result.Ko<Response<HoroscopeData>>(ZodiacSignErrors.InvalidName);
        }

        Result<(HoroscopeData Data, bool IsCached)> serviceResult =
            await horoscopeQueryService.GetOrGenerateHoroscopeAsync(
                zodiacSign, HoroscopePeriod.Daily, query.Date, cancellationToken);

        if (serviceResult.IsFailure)
        {
            return Result.Ko<Response<HoroscopeData>>(serviceResult.Errors);
        }

        (HoroscopeData data, bool isCached) = serviceResult.Value;
        Dictionary<string, string?> attributes = new() { { "cached", isCached ? "true" : "false" } };
        return Result.Ok(Response<HoroscopeData>.Create(data, attributes));
    }
}