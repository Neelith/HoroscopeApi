using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public interface ICompatibilityPromptBuilder
{
    Task<List<ChatMessage>> BuildCompatibilityMessageAsync(
        ZodiacSignInfo firstSignInfo,
        ZodiacSignInfo secondSignInfo,
        CancellationToken cancellationToken = default);
}