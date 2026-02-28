using HoroscopeApi.Application.Services.AI;
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

        // Register services
        services.AddSingleton<IHoroscopePromptBuilder, HoroscopePromptBuilder>();
        services.AddScoped<IHoroscopeGeneratorService, HoroscopeGeneratorService>();

        return services;
    }
}