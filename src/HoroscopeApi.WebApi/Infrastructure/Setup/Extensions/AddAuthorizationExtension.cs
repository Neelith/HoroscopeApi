using HoroscopeApi.WebApi.Constants;
using HoroscopeApi.WebApi.Infrastructure.Setup.Authorization.ApiKeyPolicy;
using Microsoft.AspNetCore.Authorization;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Extensions;

public static class AddAuthorizationExtension
{
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Add the API key policy
            options.AddPolicy(AuthorizationPolicies.ApiKey, policy =>
                policy.AddRequirements(new ApiKeyRequirement()));
        });

        services.AddScoped<IAuthorizationHandler, ApiKeyAuthorizationHandler>();

        return services;
    }
}