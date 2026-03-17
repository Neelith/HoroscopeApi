using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Services.CompatibilityService;

public interface ICompatibilityQueryService
{
    Task<Result<(CompatibilityData Data, bool IsCached)>> GetOrGenerateCompatibilityAsync(
        ZodiacSign firstSign,
        ZodiacSign secondSign,
        CancellationToken cancellationToken);
}