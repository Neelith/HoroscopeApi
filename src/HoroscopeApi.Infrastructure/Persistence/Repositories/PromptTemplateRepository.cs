using HoroscopeApi.Domain.Prompts;
using HoroscopeApi.Domain.Prompts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HoroscopeApi.Infrastructure.Persistence.Repositories;

internal sealed class PromptTemplateRepository(ApplicationDbContext context)
    : IPromptTemplateRepository
{
    public async Task<PromptTemplate?> GetByTypeAsync(
        PromptType type,
        CancellationToken cancellationToken)
    {
        return await context.PromptTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Type == type, cancellationToken);
    }
}