using HoroscopeApi.Domain.ApiKeys;
using HoroscopeApi.Domain.ApiKeys.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HoroscopeApi.Infrastructure.Persistence.Repositories;

internal sealed class ApiKeyRepository(ApplicationDbContext context) : IApiKeyRepository
{
    public async Task<List<Domain.ApiKeys.ApiKey>> GetByFilterAsync(
        GetApiKeysByFilterRepositoryQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<Domain.ApiKeys.ApiKey> queryable = context.ApiKeys
            .Include(a => a.Scopes)
            .AsNoTracking();

        if (query.OwnerId.HasValue)
        {
            queryable = queryable.Where(a => a.OwnerId == query.OwnerId.Value);
        }

        if (query.Type is not null)
        {
            queryable = queryable.Where(a => a.Type == query.Type.Value);
        }

        if (query.RateLimitType is not null)
        {
            queryable = queryable.Where(a => a.RateLimitType == query.RateLimitType.Value);
        }

        if (query.Ids is not null && query.Ids.Count > 0)
        {
            queryable = queryable.Where(a => query.Ids.Contains(a.Id));
        }

        if (query.Prefixes is not null && query.Prefixes.Count > 0)
        {
            queryable = queryable.Where(a => query.Prefixes.Contains(a.Prefix));
        }

        return await queryable
            .OrderBy(a => a.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertRangeAsync(
        UpsertApiKeysRepositoryCommand command,
        CancellationToken cancellationToken)
    {
        foreach (Domain.ApiKeys.ApiKey apiKey in command.ApiKeys)
        {
            Domain.ApiKeys.ApiKey? existing = await context.ApiKeys
                .Include(a => a.Scopes)
                .FirstOrDefaultAsync(a => a.Id == apiKey.Id && apiKey.Id != 0, cancellationToken);

            if (existing is not null)
            {
                existing.Prefix = apiKey.Prefix;
                existing.Hash = apiKey.Hash;
                existing.Salt = apiKey.Salt;
                existing.Algorithm = apiKey.Algorithm;
                existing.Type = apiKey.Type;
                existing.RateLimitType = apiKey.RateLimitType;
                existing.RateLimitCount = apiKey.RateLimitCount;
                existing.RateLimit = apiKey.RateLimit;

                // Remove scopes that are no longer present
                existing.Scopes.RemoveAll(s =>
                    apiKey.Scopes.All(ns => ns.Name != s.Name));

                // Add new scopes
                foreach (ApiKeyScope scope in apiKey.Scopes)
                {
                    if (existing.Scopes.All(s => s.Name != scope.Name))
                    {
                        existing.Scopes.Add(scope);
                    }
                }
            }
            else
            {
                await context.ApiKeys.AddAsync(apiKey, cancellationToken);
            }
        }
    }
}