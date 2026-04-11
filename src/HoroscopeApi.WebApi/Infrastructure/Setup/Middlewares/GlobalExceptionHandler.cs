using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.WebApi.Constants;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Middlewares;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is RateLimitExceededException rateLimitEx)
        {
            return await HandleRateLimitExceededAsync(httpContext, rateLimitEx.RateLimitResult, cancellationToken);
        }

        logger.LogError(exception, "Unhandled exception occurred");

        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Server failure"
        };

        string traceId = httpContext.Request.Headers.TryGetValue(Headers.Trace, out StringValues traceHeaderValue) &&
                         !string.IsNullOrWhiteSpace(traceHeaderValue)
            ? traceHeaderValue.ToString()
            : httpContext.TraceIdentifier;

        problemDetails.Extensions.Add("traceId", traceId);

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static async ValueTask<bool> HandleRateLimitExceededAsync(
        HttpContext httpContext,
        ApiKeyRateLimitResult rateLimitResult,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        httpContext.Response.Headers[Headers.RateLimitLimit] = rateLimitResult.Limit.ToString();
        httpContext.Response.Headers[Headers.RateLimitRemaining] = rateLimitResult.Remaining.ToString();
        httpContext.Response.Headers[Headers.RateLimitReset] = rateLimitResult.ResetAtUtc.ToString("o");

        long retryAfterSeconds = (long)Math.Ceiling(
            (rateLimitResult.ResetAtUtc - DateTime.UtcNow).TotalSeconds);
        httpContext.Response.Headers[Headers.RetryAfter] = Math.Max(1, retryAfterSeconds).ToString();

        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status429TooManyRequests,
            Type = "https://datatracker.ietf.org/doc/html/rfc6585#section-3",
            Title = "Too Many Requests"
        };

        string traceId = httpContext.Request.Headers.TryGetValue(Headers.Trace, out StringValues traceHeaderValue) &&
                         !string.IsNullOrWhiteSpace(traceHeaderValue)
            ? traceHeaderValue.ToString()
            : httpContext.TraceIdentifier;

        problemDetails.Extensions.Add("traceId", traceId);

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}