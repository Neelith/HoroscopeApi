namespace HoroscopeApi.Domain.ApiKeys.Repositories;

public sealed record UpsertApiKeysRepositoryCommand(IEnumerable<ApiKey> ApiKeys);