using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Services.CompatibilityService;

public interface ICompatibilityGeneratorService
{
    Task<Result<HuggingFaceCompatibilityData>> GenerateCompatibilityAsync(
        ZodiacSignInfo firstSignInfo,
        ZodiacSignInfo secondSignInfo,
        CancellationToken cancellationToken);
}