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

                // Add traceId property
                string traceId = httpContext.Request.Headers.TryGetValue(Headers.Trace, out StringValues traceHeader)
                    ? traceHeader.ToString()
                    : httpContext.TraceIdentifier;

                context.ProblemDetails.Extensions.TryAdd("traceId", traceId);
            });

        return services;
    }
}