using HoroscopeApi.WebApi.Infrastructure.Settings;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Extensions;

internal static class AddOpenApiExtension
{
    public static IServiceCollection AddOpenApiServices(this IServiceCollection services, JwtSettings jwtSettings)
    {
        services.AddEndpointsApiExplorer();

        services.AddOpenApi();

        return services;
    }

    public static void UseOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
                options.RoutePrefix = "openapi";
            });
        }
    }
}