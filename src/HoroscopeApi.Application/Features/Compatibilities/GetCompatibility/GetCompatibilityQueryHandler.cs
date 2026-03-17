using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.CompatibilityService;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Features.Compatibilities.GetCompatibility;

internal sealed class GetCompatibilityQueryHandler(
    ICompatibilityQueryService compatibilityQueryService)
    : IQueryHandler<GetCompatibilityQuery, Response<CompatibilityData>>
{
    public async Task<Result<Response<CompatibilityData>>> Handle(
        GetCompatibilityQuery query,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(query.FirstSignName, true, out ZodiacSign firstSign))
        {
            return Result.Ko<Response<CompatibilityData>>(ZodiacSignErrors.InvalidName);
        }

        if (!Enum.TryParse(query.SecondSignName, true, out ZodiacSign secondSign))
        {
            return Result.Ko<Response<CompatibilityData>>(ZodiacSignErrors.InvalidName);
        }

        return await compatibilityQueryService.GetOrGenerateCompatibilityAsync(
            firstSign, secondSign, cancellationToken);
    }
}