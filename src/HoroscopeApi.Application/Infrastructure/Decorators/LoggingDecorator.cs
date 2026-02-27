using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Application.Infrastructure.Decorators;

internal static class LoggingDecorator
{
    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> inner,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing command {CommandName}", typeof(TCommand).Name);

            Result result = await inner.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed command {CommandName}", typeof(TCommand).Name);
            }
            else
            {
                string errors = result.Errors.Count > 0
                    ? string.Join(", ", result.Errors.Select(e => e.Message))
                    : "Unknown error";

                logger.LogError("Completed command {CommandName} with one or more errors. Errors: {Errors}",
                    typeof(TCommand).Name,
                    errors);
            }

            return result;
        }
    }

    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : IResponse
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing command {CommandName}", typeof(TCommand).Name);

            Result<TResponse> result = await inner.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed command {CommandName}", typeof(TCommand).Name);
            }
            else
            {
                string errors = result.Errors.Count > 0
                    ? string.Join(", ", result.Errors.Select(e => e.Message))
                    : "Unknown error";

                logger.LogError("Completed command {CommandName} with one or more errors. Errors: {Errors}",
                    typeof(TCommand).Name,
                    errors);
            }

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> inner,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : IResponse
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing query {QueryName}", typeof(TQuery).Name);

            Result<TResponse> result = await inner.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed query {QueryName}", typeof(TQuery).Name);
            }
            else
            {
                string errors = result.Errors.Count > 0
                    ? string.Join(", ", result.Errors.Select(e => e.Message))
                    : "Unknown error";

                logger.LogError("Completed query {QueryName} with one or more errors. Errors: {Errors}",
                    typeof(TQuery).Name,
                    errors);
            }

            return result;
        }
    }
}