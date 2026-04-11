using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Application.Services.CompatibilityService;
using HoroscopeApi.Infrastructure.Middlewares;
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

        // Register tracing handler
        services.AddTransient<TracingDelegatingHandler>();

        // Register HttpClient for HuggingFaceClient with tracing handler
        services.AddHttpClient<IHuggingFaceClient, HuggingFaceClient>()
            .AddHttpMessageHandler<TracingDelegatingHandler>();

        // Register horoscope services
        services.AddScoped<IHoroscopePromptBuilder, HoroscopePromptBuilder>();
        services.AddScoped<IHoroscopeGeneratorService, HoroscopeGeneratorService>();

        // Register compatibility services
        services.AddScoped<ICompatibilityPromptBuilder, CompatibilityPromptBuilder>();
        services.AddScoped<ICompatibilityGeneratorService, CompatibilityGeneratorService>();

        return services;
    }
}