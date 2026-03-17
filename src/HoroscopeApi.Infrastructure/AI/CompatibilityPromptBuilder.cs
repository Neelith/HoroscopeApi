using System.Text.Json;
using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Domain.Prompts;
using HoroscopeApi.Domain.Prompts.Repositories;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public sealed class CompatibilityPromptBuilder(IPromptTemplateRepository promptTemplateRepository)
    : ICompatibilityPromptBuilder
{
    public async Task<List<ChatMessage>> BuildCompatibilityMessageAsync(
        ZodiacSignInfo firstSignInfo,
        ZodiacSignInfo secondSignInfo,
        CancellationToken cancellationToken = default)
    {
        PromptTemplate template = await GetTemplateOrThrowAsync(cancellationToken);
        return BuildMessages(template,
            new Dictionary<string, string>
            {
                { "firstSignName", firstSignInfo.Name.ToLowerInvariant() },
                { "firstElement", firstSignInfo.Element.ToString() },
                { "firstQuality", firstSignInfo.Quality.ToString() },
                { "firstRulingPlanet", firstSignInfo.RulingPlanet },
                { "firstDescription", firstSignInfo.Description },
                { "secondSignName", secondSignInfo.Name.ToLowerInvariant() },
                { "secondElement", secondSignInfo.Element.ToString() },
                { "secondQuality", secondSignInfo.Quality.ToString() },
                { "secondRulingPlanet", secondSignInfo.RulingPlanet },
                { "secondDescription", secondSignInfo.Description }
            });
    }

    private async Task<PromptTemplate> GetTemplateOrThrowAsync(CancellationToken cancellationToken)
    {
        PromptTemplate? template =
            await promptTemplateRepository.GetByTypeAsync(PromptType.Compatibility, cancellationToken);

        if (template is null)
        {
            throw new InvalidOperationException(
                "No prompt template found for type 'Compatibility'. Ensure the database has been seeded.");
        }

        return template;
    }

    private static List<ChatMessage> BuildMessages(
        PromptTemplate template,
        Dictionary<string, string> placeholders)
    {
        List<ChatMessage> messages = new();

        // System message
        messages.Add(new ChatMessage("system", template.SystemPrompt));

        // Few-shot examples from JSON
        if (!string.IsNullOrWhiteSpace(template.FewShotExamples))
        {
            List<ChatMessage>? examples = JsonSerializer.Deserialize<List<ChatMessage>>(
                template.FewShotExamples,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (examples is not null)
            {
                messages.AddRange(examples);
            }
        }

        // User prompt with placeholders replaced
        string userPrompt = template.UserPromptTemplate;
        foreach (KeyValuePair<string, string> placeholder in placeholders)
        {
            userPrompt = userPrompt.Replace($"{{{placeholder.Key}}}", placeholder.Value);
        }

        messages.Add(new ChatMessage("user", userPrompt));

        return messages;
    }
}