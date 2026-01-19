using Serilog.Context;
using HoroscopeApi.WebApi.Constants;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Middlewares;

public class TraceMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string traceId = context.Request.Headers.TryGetValue(Headers.Trace, out var traceHeaderValue) && !string.IsNullOrWhiteSpace(traceHeaderValue)
            ? traceHeaderValue.ToString()
            : context.TraceIdentifier;

        context.Response.Headers.TryAdd(Headers.Trace, traceId);

        using var logcontext = LogContext.PushProperty("TraceIdentifier", traceId);

        await next(context);
    }
}
