using System.Diagnostics;
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

        string traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;
        string spanId = Activity.Current?.SpanId.ToString() ?? "";
        string correlationId = httpContext.Request.Headers.TryGetValue(Headers.Correlation, out StringValues correlationHeader) &&
                               !string.IsNullOrWhiteSpace(correlationHeader)
            ? correlationHeader.ToString()
            : Guid.NewGuid().ToString();
        string? tracestate = httpContext.Request.Headers.TryGetValue(Headers.TraceState, out StringValues tracestateHeader) &&
                             !string.IsNullOrWhiteSpace(tracestateHeader)
            ? tracestateHeader.ToString()
            : null;

        problemDetails.Extensions.Add("traceId", traceId);
        problemDetails.Extensions.Add("spanId", spanId);
        problemDetails.Extensions.Add("correlationId", correlationId);

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        if (!string.IsNullOrEmpty(tracestate))
        {
            httpContext.Response.Headers.TryAdd(Headers.TraceState, tracestate);
        }

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

        string traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;
        string spanId = Activity.Current?.SpanId.ToString() ?? "";
        string correlationId = httpContext.Request.Headers.TryGetValue(Headers.Correlation, out StringValues correlationHeader) &&
                               !string.IsNullOrWhiteSpace(correlationHeader)
            ? correlationHeader.ToString()
            : Guid.NewGuid().ToString();
        string? tracestate = httpContext.Request.Headers.TryGetValue(Headers.TraceState, out StringValues tracestateHeader) &&
                             !string.IsNullOrWhiteSpace(tracestateHeader)
            ? tracestateHeader.ToString()
            : null;

        problemDetails.Extensions.Add("traceId", traceId);
        problemDetails.Extensions.Add("spanId", spanId);
        problemDetails.Extensions.Add("correlationId", correlationId);

        if (!string.IsNullOrEmpty(tracestate))
        {
            httpContext.Response.Headers.TryAdd(Headers.TraceState, tracestate);
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}