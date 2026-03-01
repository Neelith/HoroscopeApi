namespace HoroscopeApi.Domain.ApiKeys.Repositories;

public interface IApiKeyRepository
{
    Task<List<ApiKey>> GetByFilterAsync(
        GetApiKeysByFilterRepositoryQuery query,
        CancellationToken cancellationToken);

    Task UpsertRangeAsync(
        UpsertApiKeysRepositoryCommand command,
        CancellationToken cancellationToken);
}