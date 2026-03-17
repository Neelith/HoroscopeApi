using HoroscopeApi.Domain.Constants;

namespace HoroscopeApi.Domain.Prompts;

public static class PromptTemplateErrors
{
    public static Error InvalidPromptTemplate => new(
        "PromptTemplate.InvalidPromptTemplate",
        "Prompt template system prompt and user prompt template are required.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode } }
    };

    public static Error NotFound => new(
        "PromptTemplate.NotFound",
        "No prompt template found for the specified type.")
    {
        Metadata = new Dictionary<string, string?> { { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode } }
    };
}