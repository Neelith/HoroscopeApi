using System.Reflection;
using HoroscopeApi.Application.Infrastructure.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace HoroscopeApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //Register application services here
        Assembly assembly = typeof(DependencyInjection).Assembly;

        //Register handlers and validators
        services
            .AddHandlers()
            .AddValidatorsFromAssembly(assembly)
            .AddDecorators();

        return services;
    }
}