using System.Text.Json;
using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Domain.Prompts;
using HoroscopeApi.Domain.Prompts.Repositories;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public sealed class HoroscopePromptBuilder(IPromptTemplateRepository promptTemplateRepository)
    : IHoroscopePromptBuilder
{
    public async Task<List<ChatMessage>> BuildMessageAsync(
        DateOnly date,
        ZodiacSignInfo zodiacSignInfo,
        CancellationToken cancellationToken = default)
    {
        PromptTemplate template = await GetTemplateOrThrowAsync(PromptType.Daily, cancellationToken);
        return BuildMessages(template,
            new Dictionary<string, string>
            {
                { "signName", zodiacSignInfo.Name.ToLowerInvariant() },
                { "date", date.ToString("yyyy-MM-dd") },
                { "element", zodiacSignInfo.Element.ToString() },
                { "quality", zodiacSignInfo.Quality.ToString() },
                { "rulingPlanet", zodiacSignInfo.RulingPlanet },
                { "description", zodiacSignInfo.Description }
            });
    }

    public async Task<List<ChatMessage>> BuildWeeklyMessageAsync(
        DateOnly date,
        ZodiacSignInfo zodiacSignInfo,
        CancellationToken cancellationToken = default)
    {
        PromptTemplate template = await GetTemplateOrThrowAsync(PromptType.Weekly, cancellationToken);
        return BuildMessages(template,
            new Dictionary<string, string>
            {
                { "signName", zodiacSignInfo.Name.ToLowerInvariant() },
                { "date", date.ToString("yyyy-MM-dd") },
                { "element", zodiacSignInfo.Element.ToString() },
                { "quality", zodiacSignInfo.Quality.ToString() },
                { "rulingPlanet", zodiacSignInfo.RulingPlanet },
                { "description", zodiacSignInfo.Description }
            });
    }

    public async Task<List<ChatMessage>> BuildMonthlyMessageAsync(
        DateOnly date,
        ZodiacSignInfo zodiacSignInfo,
        CancellationToken cancellationToken = default)
    {
        PromptTemplate template = await GetTemplateOrThrowAsync(PromptType.Monthly, cancellationToken);
        string monthName = date.ToString("MMMM");
        int year = date.Year;
        return BuildMessages(template,
            new Dictionary<string, string>
            {
                { "signName", zodiacSignInfo.Name.ToLowerInvariant() },
                { "date", date.ToString("yyyy-MM-dd") },
                { "monthName", monthName },
                { "year", year.ToString() },
                { "element", zodiacSignInfo.Element.ToString() },
                { "quality", zodiacSignInfo.Quality.ToString() },
                { "rulingPlanet", zodiacSignInfo.RulingPlanet },
                { "description", zodiacSignInfo.Description }
            });
    }

    public async Task<List<ChatMessage>> BuildYearlyMessageAsync(
        int year,
        ZodiacSignInfo zodiacSignInfo,
        CancellationToken cancellationToken = default)
    {
        PromptTemplate template = await GetTemplateOrThrowAsync(PromptType.Yearly, cancellationToken);
        return BuildMessages(template,
            new Dictionary<string, string>
            {
                { "signName", zodiacSignInfo.Name.ToLowerInvariant() },
                { "year", year.ToString() },
                { "element", zodiacSignInfo.Element.ToString() },
                { "quality", zodiacSignInfo.Quality.ToString() },
                { "rulingPlanet", zodiacSignInfo.RulingPlanet },
                { "description", zodiacSignInfo.Description }
            });
    }

    private async Task<PromptTemplate> GetTemplateOrThrowAsync(
        PromptType type,
        CancellationToken cancellationToken)
    {
        PromptTemplate? template =
            await promptTemplateRepository.GetByTypeAsync(type, cancellationToken);

        if (template is null)
        {
            throw new InvalidOperationException(
                $"No prompt template found for type '{type}'. Ensure the database has been seeded.");
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