using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Services.HoroscopeService;

public interface IHoroscopeQueryService
{
    Task<Result<Response<HoroscopeData>>> GetOrGenerateHoroscopeAsync(
        ZodiacSign zodiacSign,
        HoroscopePeriod period,
        DateOnly date,
        CancellationToken cancellationToken);
}