using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HoroscopeApi.WebApi.Constants;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Middlewares;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred");

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Server failure"
        };

        string traceId = httpContext.Request.Headers.TryGetValue(Headers.Trace, out var traceHeaderValue) && !string.IsNullOrWhiteSpace(traceHeaderValue)
            ? traceHeaderValue.ToString()
            : httpContext.TraceIdentifier;

        problemDetails.Extensions.Add("traceId", traceId);

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}