namespace HoroscopeApi.Domain.ZodiacSigns.Repositories;

public interface IZodiacSignRepository
{
    Task<List<ZodiacSignInfo>> GetAllAsync(CancellationToken cancellationToken);
    Task<ZodiacSignInfo?> GetBySignAsync(GetZodiacSignBySignRepositoryQuery query, CancellationToken cancellationToken);
    Task<ZodiacSignInfo?> GetByDateAsync(GetZodiacSignByDateRepositoryQuery query, CancellationToken cancellationToken);
}
