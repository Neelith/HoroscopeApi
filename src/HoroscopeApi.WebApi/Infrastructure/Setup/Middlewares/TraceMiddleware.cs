using HoroscopeApi.WebApi.Constants;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Middlewares;

public class TraceMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string traceId = context.Request.Headers.TryGetValue(Headers.Trace, out StringValues traceHeaderValue) &&
                         !string.IsNullOrWhiteSpace(traceHeaderValue)
            ? traceHeaderValue.ToString()
            : context.TraceIdentifier;

        context.Response.Headers.TryAdd(Headers.Trace, traceId);

        using IDisposable logcontext = LogContext.PushProperty("TraceIdentifier", traceId);

        await next(context);
    }
}