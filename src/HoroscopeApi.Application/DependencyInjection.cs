using Microsoft.Extensions.DependencyInjection;
using HoroscopeApi.Application.Infrastructure.Handlers;

namespace HoroscopeApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //Register application services here
        var assembly = typeof(DependencyInjection).Assembly;

        //Register handlers and validators
        services
            .AddHandlers()
            .AddValidatorsFromAssembly(assembly)
            .AddDecorators();

        return services;
    }
}
