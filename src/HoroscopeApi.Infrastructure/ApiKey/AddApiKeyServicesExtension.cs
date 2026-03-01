using HoroscopeApi.Application.Services.ApiKey;
using Microsoft.Extensions.DependencyInjection;

namespace HoroscopeApi.Infrastructure.ApiKey;

public static class AddApiKeyServicesExtension
{
    public static IServiceCollection AddApiKeyServices(this IServiceCollection services)
    {
        services.AddScoped<IApiKeyService, ApiKeyService>();

        return services;
    }
}