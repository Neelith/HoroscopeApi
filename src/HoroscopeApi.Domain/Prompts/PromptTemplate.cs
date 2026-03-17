using HoroscopeApi.Domain.Shared;

namespace HoroscopeApi.Domain.Prompts;

public sealed class PromptTemplate : AuditableEntity
{
    // EF constructor
    private PromptTemplate() { }

    private PromptTemplate(
        PromptType type,
        string systemPrompt,
        string fewShotExamples,
        string userPromptTemplate)
    {
        Type = type;
        SystemPrompt = systemPrompt;
        FewShotExamples = fewShotExamples;
        UserPromptTemplate = userPromptTemplate;
    }

    public int Id { get; private set; }
    public PromptType Type { get; private set; }
    public string SystemPrompt { get; private set; } = string.Empty;
    public string FewShotExamples { get; private set; } = string.Empty;
    public string UserPromptTemplate { get; private set; } = string.Empty;

    public static Result<PromptTemplate> Create(
        PromptType type,
        string systemPrompt,
        string fewShotExamples,
        string userPromptTemplate)
    {
        if (string.IsNullOrWhiteSpace(systemPrompt))
        {
            return Result.Ko<PromptTemplate>(PromptTemplateErrors.InvalidPromptTemplate);
        }

        if (string.IsNullOrWhiteSpace(userPromptTemplate))
        {
            return Result.Ko<PromptTemplate>(PromptTemplateErrors.InvalidPromptTemplate);
        }

        PromptTemplate template = new(type, systemPrompt, fewShotExamples, userPromptTemplate);
        return Result.Ok(template);
    }
}