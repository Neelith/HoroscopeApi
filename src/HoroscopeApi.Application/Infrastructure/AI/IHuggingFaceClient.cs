using Hermes.Results;

namespace HoroscopeApi.Application.Infrastructure.AI;

public interface IHuggingFaceClient
{
    Task<Result<HuggingFaceHoroscopeData>> GenerateHoroscopeAsync(
        GenerateHoroscopeRequest request,
        CancellationToken cancellationToken = default);
    
    Task<Result<HuggingFaceHoroscopeData>> GenerateWithMessages(
        List<ChatMessage> messages,
        CancellationToken cancellationToken = default);
}
