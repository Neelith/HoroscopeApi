using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Application.Services.CompatibilityService;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public sealed class CompatibilityGeneratorService(
    IHuggingFaceClient huggingFaceClient,
    ICompatibilityPromptBuilder promptBuilder)
    : ICompatibilityGeneratorService
{
    public async Task<Result<HuggingFaceCompatibilityData>> GenerateCompatibilityAsync(
        ZodiacSignInfo firstSignInfo,
        ZodiacSignInfo secondSignInfo,
        CancellationToken cancellationToken)
    {
        List<ChatMessage> messages = await promptBuilder.BuildCompatibilityMessageAsync(
            firstSignInfo, secondSignInfo, cancellationToken);

        return await huggingFaceClient.GenerateCompatibilityWithMessages(messages, cancellationToken);
    }
}