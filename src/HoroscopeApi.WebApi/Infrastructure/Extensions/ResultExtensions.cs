using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using HoroscopeApi.Shared.Constants;

namespace HoroscopeApi.WebApi.Infrastructure.Extensions;

internal static class ResultExtensions
{

    public static ProblemHttpResult ToErrorResponse<T>(this Result<T> result)
    {
        return ToErrorResponse(result as Result);
    }

    public static ProblemHttpResult ToErrorResponse(this Result result)
    {
        if (result is null || result.IsSuccess)
        {
            throw new ArgumentException("Expected 'failed' result, but 'success' result was found instead");
        }

        string? errorType = default;

        var errorTypeParsed = result.Metadata?.TryGetValue(ErrorConsts.ErrorType, out errorType);

        if (errorTypeParsed is not true || errorType is null)
        {
            return result.ToProblem(HttpStatusCode.InternalServerError);
        }

        return errorType switch
        {
            ErrorConsts.BadRequestCode => result.ToProblem(HttpStatusCode.BadRequest),
            ErrorConsts.NotFoundCode => result.ToProblem(HttpStatusCode.NotFound),
            ErrorConsts.InternalServerErrorCode => result.ToProblem(HttpStatusCode.InternalServerError),
            _ => throw new ArgumentException("Unhandled result error code"),
        };
    }

    private static ProblemHttpResult ToProblem(this Result result, HttpStatusCode statusCode)
    {
        var errors = result.Errors.Count > 1
            ? string.Join("\n---\n", result.Errors.Select(e => e.Message))
            : result.Errors.Count == 0 ? "Generic error." : result.Errors[0].Message;

        return TypedResults.Problem(detail: errors, statusCode: (int)statusCode, title: statusCode.ToString());
    }
}
