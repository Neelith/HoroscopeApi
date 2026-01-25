using Microsoft.EntityFrameworkCore;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.Persistence.Repositories;

internal sealed class HoroscopeRepository(ApplicationDbContext context) : IHoroscopeRepository
{
    public async Task<Horoscope?> GetBySignAndPeriodAsync(
        GetHoroscopeBySignAndPeriodRepositoryQuery query,
        CancellationToken cancellationToken)
    {
        return await context.Horoscopes
            .Include(h => h.ZodiacSignInfo)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                h => h.ZodiacSignId == query.ZodiacSignId && h.Period == query.Period && h.Date == query.Date,
                cancellationToken);
    }

    public async Task<List<Horoscope>> GetByDateRangeAsync(
        GetHoroscopesByDateRangeRepositoryQuery query,
        CancellationToken cancellationToken)
    {
        return await context.Horoscopes
            .Include(h => h.ZodiacSignInfo)
            .AsNoTracking()
            .Where(h => h.ZodiacSignId == query.ZodiacSignId
                && h.Period == query.Period
                && h.Date >= query.StartDate
                && h.Date <= query.EndDate)
            .OrderBy(h => h.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Horoscope horoscope, CancellationToken cancellationToken)
    {
        await context.Horoscopes.AddAsync(horoscope, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Horoscope> horoscopes, CancellationToken cancellationToken)
    {
        await context.Horoscopes.AddRangeAsync(horoscopes, cancellationToken);
    }
}
