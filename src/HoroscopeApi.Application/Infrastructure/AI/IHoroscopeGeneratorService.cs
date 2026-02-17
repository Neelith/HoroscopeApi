using Hermes.Results;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Infrastructure.AI;

public interface IHoroscopeGeneratorService
{
    Task<Result<Horoscope>> GenerateDailyHoroscopeAsync(
        ZodiacSign sign,
        DateOnly date,
        CancellationToken cancellationToken = default);
    
    Task<Result<Horoscope>> GenerateYearlyHoroscopeAsync(
        ZodiacSign sign,
        int year,
        CancellationToken cancellationToken = default);
}
