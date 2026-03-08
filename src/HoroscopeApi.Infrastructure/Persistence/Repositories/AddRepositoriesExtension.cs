using HoroscopeApi.Domain.ApiKeys.Repositories;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HoroscopeApi.Infrastructure.Persistence.Repositories;

internal static class AddRepositoriesExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IZodiacSignRepository, ZodiacSignRepository>();
        services.AddScoped<IHoroscopeRepository, HoroscopeRepository>();
        services.AddScoped<IHoroscopePromptTemplateRepository, HoroscopePromptTemplateRepository>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();

        return services;
    }
}