namespace HoroscopeApi.Domain.Compatibilities.Repositories;

public interface ICompatibilityRepository
{
    Task<Compatibility?> GetBySignPairAsync(
        GetCompatibilityBySignPairRepositoryQuery query,
        CancellationToken cancellationToken);

    Task AddAsync(Compatibility compatibility, CancellationToken cancellationToken);
}