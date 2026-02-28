using System.Net;
using HoroscopeApi.Domain.Constants;

namespace HoroscopeApi.WebApi.Infrastructure.Extensions;

internal static class ResultExtensions
{
    public static IResult ToErrorResponse<T>(this Result<T> result)
    {
        return (result as Result).ToErrorResponse();
    }

    public static IResult ToErrorResponse(this Result result)
    {
        if (result is null || result.IsSuccess)
        {
            throw new ArgumentException("Expected 'failed' result, but 'success' result was found instead");
        }

        // Check metadata from the first error (all errors should have the same HTTP status)
        string? errorType = null;
        if (result.Errors.Count > 0 && result.Errors[0].Metadata != null)
        {
            result.Errors[0]!.Metadata!.TryGetValue(ErrorConsts.ErrorType, out errorType);
        }

        // Fallback to result-level metadata if error doesn't have it
        if (errorType is null && result.Metadata != null)
        {
            result.Metadata.TryGetValue(ErrorConsts.ErrorType, out errorType);
        }

        if (errorType is null)
        {
            return result.ToProblem(HttpStatusCode.InternalServerError);
        }

        return errorType switch
        {
            ErrorConsts.BadRequestCode => result.ToProblem(HttpStatusCode.BadRequest),
            ErrorConsts.NotFoundCode => result.ToProblem(HttpStatusCode.NotFound),
            ErrorConsts.InternalServerErrorCode => result.ToProblem(HttpStatusCode.InternalServerError),
            _ => throw new ArgumentException("Unhandled result error code")
        };
    }

    private static IResult ToProblem(this Result result, HttpStatusCode statusCode)
    {
        string errors = result.Errors.Count > 1
            ? string.Join("\n---\n", result.Errors.Select(e => e.Message))
            : result.Errors.Count == 0
                ? "Generic error."
                : result.Errors[0].Message;

        return TypedResults.Problem(errors, statusCode: (int)statusCode, title: statusCode.ToString());
    }
}