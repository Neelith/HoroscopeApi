using HoroscopeApi.Domain.ApiKeys.Repositories;
using HoroscopeApi.Domain.Compatibilities.Repositories;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using HoroscopeApi.Domain.Prompts.Repositories;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HoroscopeApi.Infrastructure.Persistence.Repositories;

internal static class AddRepositoriesExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IZodiacSignRepository, ZodiacSignRepository>();
        services.AddScoped<IHoroscopeRepository, HoroscopeRepository>();
        services.AddScoped<ICompatibilityRepository, CompatibilityRepository>();
        services.AddScoped<IPromptTemplateRepository, PromptTemplateRepository>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();

        return services;
    }
}