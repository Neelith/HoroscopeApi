using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Domain.Horoscopes.Repositories;

public interface IHoroscopeRepository
{
    Task<Horoscope?> GetBySignAndPeriodAsync(
        GetHoroscopeBySignAndPeriodRepositoryQuery query,
        CancellationToken cancellationToken);

    Task<List<Horoscope>> GetByDateRangeAsync(
        GetHoroscopesByDateRangeRepositoryQuery query,
        CancellationToken cancellationToken);

    Task AddAsync(Horoscope horoscope, CancellationToken cancellationToken);
}
