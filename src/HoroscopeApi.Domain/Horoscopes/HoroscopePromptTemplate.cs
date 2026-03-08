using HoroscopeApi.Domain.Shared;

namespace HoroscopeApi.Domain.Horoscopes;

public sealed class HoroscopePromptTemplate : AuditableEntity
{
    // EF constructor
    private HoroscopePromptTemplate() { }

    private HoroscopePromptTemplate(
        HoroscopePeriod period,
        string systemPrompt,
        string fewShotExamples,
        string userPromptTemplate)
    {
        Period = period;
        SystemPrompt = systemPrompt;
        FewShotExamples = fewShotExamples;
        UserPromptTemplate = userPromptTemplate;
    }

    public int Id { get; private set; }
    public HoroscopePeriod Period { get; private set; }
    public string SystemPrompt { get; private set; } = string.Empty;
    public string FewShotExamples { get; private set; } = string.Empty;
    public string UserPromptTemplate { get; private set; } = string.Empty;

    public static Result<HoroscopePromptTemplate> Create(
        HoroscopePeriod period,
        string systemPrompt,
        string fewShotExamples,
        string userPromptTemplate)
    {
        if (string.IsNullOrWhiteSpace(systemPrompt))
        {
            return Result.Ko<HoroscopePromptTemplate>(HoroscopeErrors.InvalidPromptTemplate);
        }

        if (string.IsNullOrWhiteSpace(userPromptTemplate))
        {
            return Result.Ko<HoroscopePromptTemplate>(HoroscopeErrors.InvalidPromptTemplate);
        }

        HoroscopePromptTemplate template = new(period, systemPrompt, fewShotExamples, userPromptTemplate);
        return Result.Ok(template);
    }
}
