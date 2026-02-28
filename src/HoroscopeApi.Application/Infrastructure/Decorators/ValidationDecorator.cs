using FluentValidation.Results;
using HoroscopeApi.Domain.Constants;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace HoroscopeApi.Application.Infrastructure.Decorators;

internal static class ValidationDecorator
{
    private static async Task<ValidationFailure[]> ValidateAndCollectFailures<T>(
        T request,
        IEnumerable<IValidator<T>> validators,
        CancellationToken cancellationToken)
    {
        ValidationContext<T> context = new(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        ValidationFailure[] failures =
        [
            .. validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
        ];

        return failures;
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> inner,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] failures = await ValidateAndCollectFailures(command, validators, cancellationToken);

            if (failures.Length != 0)
            {
                Error[] errors = failures.Select(f =>
                    new Error(f.ErrorCode ?? "VALIDATION_ERROR", f.ErrorMessage)
                    {
                        Metadata = new Dictionary<string, string?>
                        {
                            { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode }
                        }
                    }).ToArray();

                return Result.Ko(errors);
            }

            return await inner.Handle(command, cancellationToken);
        }
    }

    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : IResponse
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] failures = await ValidateAndCollectFailures(command, validators, cancellationToken);

            if (failures.Length != 0)
            {
                Error[] errors = failures.Select(f =>
                    new Error(f.ErrorCode ?? "VALIDATION_ERROR", f.ErrorMessage)
                    {
                        Metadata = new Dictionary<string, string?>
                        {
                            { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode }
                        }
                    }).ToArray();

                return Result.Ko<TResponse>(errors);
            }

            return await inner.Handle(command, cancellationToken);
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> inner,
        IEnumerable<IValidator<TQuery>> validators)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : IResponse
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            ValidationFailure[] failures = await ValidateAndCollectFailures(query, validators, cancellationToken);

            if (failures.Length != 0)
            {
                Error[] errors = failures.Select(f =>
                    new Error(f.ErrorCode ?? "VALIDATION_ERROR", f.ErrorMessage)
                    {
                        Metadata = new Dictionary<string, string?>
                        {
                            { ErrorConsts.ErrorType, ErrorConsts.BadRequestCode }
                        }
                    }).ToArray();

                return Result.Ko<TResponse>(errors);
            }

            return await inner.Handle(query, cancellationToken);
        }
    }
}