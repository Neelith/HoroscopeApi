using Microsoft.EntityFrameworkCore;
using HoroscopeApi.Domain.ZodiacSigns;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;

namespace HoroscopeApi.Infrastructure.Persistence.Repositories;

internal sealed class ZodiacSignRepository(ApplicationDbContext context) : IZodiacSignRepository
{
    public async Task<List<ZodiacSignInfo>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.ZodiacSigns
            .AsNoTracking()
            .OrderBy(z => z.Sign)
            .ToListAsync(cancellationToken);
    }

    public async Task<ZodiacSignInfo?> GetBySignAsync(GetZodiacSignBySignRepositoryQuery query, CancellationToken cancellationToken)
    {
        return await context.ZodiacSigns
            .AsNoTracking()
            .FirstOrDefaultAsync(z => z.Sign == query.Sign, cancellationToken);
    }

    public async Task<ZodiacSignInfo?> GetByDateAsync(GetZodiacSignByDateRepositoryQuery query, CancellationToken cancellationToken)
    {
        var allSigns = await GetAllAsync(cancellationToken);
        return allSigns.FirstOrDefault(sign => sign.IsDateInRange(query.Month, query.Day));
    }
}
