using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Application.Services.CompatibilityService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HoroscopeApi.Infrastructure.AI;

public static class AddHuggingFaceExtension
{
    public static IServiceCollection AddHuggingFace(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register settings
        IConfigurationSection settingsSection = configuration.GetSection("HuggingFace");
        services.Configure<HuggingFaceSettings>(settingsSection);

        // Register HttpClient for HuggingFaceClient
        services.AddHttpClient<IHuggingFaceClient, HuggingFaceClient>();

        // Register horoscope services
        services.AddScoped<IHoroscopePromptBuilder, HoroscopePromptBuilder>();
        services.AddScoped<IHoroscopeGeneratorService, HoroscopeGeneratorService>();

        // Register compatibility services
        services.AddScoped<ICompatibilityPromptBuilder, CompatibilityPromptBuilder>();
        services.AddScoped<ICompatibilityGeneratorService, CompatibilityGeneratorService>();

        return services;
    }
}