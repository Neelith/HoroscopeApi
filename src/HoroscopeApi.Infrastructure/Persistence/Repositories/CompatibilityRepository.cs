using HoroscopeApi.Domain.Compatibilities;
using HoroscopeApi.Domain.Compatibilities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HoroscopeApi.Infrastructure.Persistence.Repositories;

internal sealed class CompatibilityRepository(ApplicationDbContext context) : ICompatibilityRepository
{
    public async Task<Compatibility?> GetBySignPairAsync(
        GetCompatibilityBySignPairRepositoryQuery query,
        CancellationToken cancellationToken)
    {
        return await context.Compatibilities
            .Include(c => c.FirstZodiacSignInfo)
            .Include(c => c.SecondZodiacSignInfo)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.FirstZodiacSignId == query.FirstZodiacSignId
                     && c.SecondZodiacSignId == query.SecondZodiacSignId,
                cancellationToken);
    }

    public async Task AddAsync(Compatibility compatibility, CancellationToken cancellationToken)
    {
        await context.Compatibilities.AddAsync(compatibility, cancellationToken);
    }
}