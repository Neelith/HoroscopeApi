using System.Diagnostics;
using HoroscopeApi.WebApi.Constants;
using Microsoft.Extensions.Primitives;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Extensions;

internal static class AddProblemDetailsExtension
{
    public static IServiceCollection ConfigureProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = context =>
            {
                HttpContext httpContext = context.HttpContext;

                PathString instance = httpContext.Request.Path;
                context.ProblemDetails.Instance = instance;

                if (context.Exception is null)
                {
                    string method = httpContext.Request.Method;
                    context.ProblemDetails.Extensions.TryAdd("method", method);

                    context.ProblemDetails.Extensions.TryAdd("endpoint", $"{method} {instance}");
                }

                string traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;
                string spanId = Activity.Current?.SpanId.ToString() ?? "";
                string correlationId = httpContext.Request.Headers.TryGetValue(Headers.Correlation, out StringValues correlationHeader) &&
                                        !string.IsNullOrWhiteSpace(correlationHeader)
                    ? correlationHeader.ToString()
                    : Guid.NewGuid().ToString();

                context.ProblemDetails.Extensions.TryAdd("traceId", traceId);
                context.ProblemDetails.Extensions.TryAdd("spanId", spanId);
                context.ProblemDetails.Extensions.TryAdd("correlationId", correlationId);
            });

        return services;
    }
}