using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Services.AI;

public interface IHoroscopeGeneratorService
{
    Task<Result<Horoscope>> GenerateDailyHoroscopeAsync(
        ZodiacSign sign,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<Result<Horoscope>> GenerateWeeklyHoroscopeAsync(
        ZodiacSign sign,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<Result<Horoscope>> GenerateMonthlyHoroscopeAsync(
        ZodiacSign sign,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<Result<Horoscope>> GenerateYearlyHoroscopeAsync(
        ZodiacSign sign,
        int year,
        CancellationToken cancellationToken = default);
}