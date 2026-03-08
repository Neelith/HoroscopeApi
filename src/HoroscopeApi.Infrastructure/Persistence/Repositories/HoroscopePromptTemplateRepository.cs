using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HoroscopeApi.Infrastructure.Persistence.Repositories;

internal sealed class HoroscopePromptTemplateRepository(ApplicationDbContext context)
    : IHoroscopePromptTemplateRepository
{
    public async Task<HoroscopePromptTemplate?> GetByPeriodAsync(
        HoroscopePeriod period,
        CancellationToken cancellationToken)
    {
        return await context.HoroscopePromptTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Period == period, cancellationToken);
    }
}
