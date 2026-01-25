using Hermes.Results;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Infrastructure.AI;

public interface IHuggingFaceClient
{
    Task<Result<HuggingFaceBatchResponse>> GenerateBatchHoroscopesAsync(
        GenerateBatchHoroscopesRequest request,
        CancellationToken cancellationToken = default);
}
