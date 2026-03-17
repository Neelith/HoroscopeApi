namespace HoroscopeApi.Domain.Prompts.Repositories;

public interface IPromptTemplateRepository
{
    Task<PromptTemplate?> GetByTypeAsync(
        PromptType type,
        CancellationToken cancellationToken);
}