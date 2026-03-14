using HoroscopeApi.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Extensions;

public static class AddAuthenticationExtension
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services,
        JwtSettings? jwtSettings)
    {
        if (jwtSettings is null)
        {
            throw new ArgumentNullException(nameof(jwtSettings), "JWT settings must be provided.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.Authority = jwtSettings.Authority;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience
                };
            });

        return services;
    }
}