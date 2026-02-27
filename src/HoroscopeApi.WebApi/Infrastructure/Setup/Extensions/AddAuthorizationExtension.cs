namespace HoroscopeApi.WebApi.Infrastructure.Setup.Extensions;

public static class AddAuthorizationExtension
{
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddAuthorization();

        return services;
    }
}