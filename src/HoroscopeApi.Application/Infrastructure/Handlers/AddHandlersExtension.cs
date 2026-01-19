using Microsoft.Extensions.DependencyInjection;
using HoroscopeApi.Application.Infrastructure.Decorators;

namespace HoroscopeApi.Application.Infrastructure.Handlers;

public static class AddHandlersExtension
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        //Register the query handlers
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddHandlers([assembly]);

        return services;
    }

    public static IServiceCollection AddDecorators(this IServiceCollection services)
    {
        //Here we decorate the command handlers with the validation decorator
        //This decorator will validate the command before executing it
        services.AddHandlerDecorator(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        services.AddHandlerDecorator(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));
        services.AddHandlerDecorator(typeof(IQueryHandler<,>), typeof(ValidationDecorator.QueryHandler<,>));

        //Here we decorate the handlers with the logging decorator
        //This decorator will log the handler before and after executing it
        services.AddHandlerDecorator(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        services.AddHandlerDecorator(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));
        services.AddHandlerDecorator(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));

        return services;
    }
}