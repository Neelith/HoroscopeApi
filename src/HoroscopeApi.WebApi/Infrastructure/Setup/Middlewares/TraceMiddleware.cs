using System.Diagnostics;
using HoroscopeApi.WebApi.Constants;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Middlewares;

public class TraceMiddleware(RequestDelegate next)
{
    private const int TraceIdLength = 32;
    private const int ParentIdLength = 16;
    private const string ValidVersion = "00";

    public async Task InvokeAsync(HttpContext context)
    {
        string traceId;
        string parentId;
        string traceFlags;
        string? incomingTracestate;

        string? incomingTraceparent = GetHeaderValue(context.Request.Headers, Headers.TraceParent);
        incomingTracestate = GetHeaderValue(context.Request.Headers, Headers.TraceState);

        if (incomingTraceparent != null)
        {
            var parseResult = ParseTraceparent(incomingTraceparent);
            if (parseResult.IsValid)
            {
                traceId = parseResult.TraceId;
                parentId = parseResult.ParentId;
                traceFlags = parseResult.TraceFlags;
            }
            else
            {
                (traceId, parentId, traceFlags) = RestartTrace();
                incomingTracestate = null;
            }
        }
        else
        {
            (traceId, parentId, traceFlags) = RestartTrace();
            incomingTracestate = null;
        }

        var activity = new Activity("HoroscopeApi.Request")
            .SetParentId(incomingTraceparent ?? $"00-{traceId}-{parentId}-{traceFlags}");
        activity.Start();

        string currentTraceId = activity.TraceId.ToString();
        string currentSpanId = activity.SpanId.ToString();
        
        string correlationId = GetHeaderValue(context.Request.Headers, Headers.Correlation) 
            ?? Guid.NewGuid().ToString();

        string outgoingTraceparent = $"00-{currentTraceId}-{currentSpanId}-{traceFlags}";
        context.Response.Headers.TryAdd(Headers.TraceParent, outgoingTraceparent);
        context.Response.Headers.TryAdd(Headers.Correlation, correlationId);

        if (!string.IsNullOrEmpty(incomingTracestate))
        {
            context.Response.Headers.TryAdd(Headers.TraceState, incomingTracestate);
        }

        using IDisposable _ = LogContext.PushProperty("TraceId", currentTraceId);
        using IDisposable __ = LogContext.PushProperty("ParentId", parentId);
        using IDisposable ___ = LogContext.PushProperty("SpanId", currentSpanId);
        using IDisposable ____ = LogContext.PushProperty("CorrelationId", correlationId);
        using IDisposable _____ = LogContext.PushProperty("TraceFlags", traceFlags);

        await next(context);

        activity.Stop();
    }

    private static string? GetHeaderValue(IHeaderDictionary headers, string key)
    {
        if (headers.TryGetValue(key, out StringValues value) && !string.IsNullOrWhiteSpace(value))
        {
            return value.ToString();
        }
        return null;
    }

    private static (bool IsValid, string TraceId, string ParentId, string TraceFlags) ParseTraceparent(string traceparent)
    {
        if (string.IsNullOrWhiteSpace(traceparent))
            return (false, "", "", "");

        var parts = traceparent.Split('-');
        if (parts.Length < 4)
            return (false, "", "", "");

        string version = parts[0];
        string traceId = parts[1];
        string parentId = parts[2];
        string traceFlags = parts[3];

        if (!IsValidVersion(version))
            return (false, "", "", "");

        if (!IsValidTraceId(traceId))
            return (false, "", "", "");

        if (!IsValidParentId(parentId))
            return (false, "", "", "");

        return (true, traceId, parentId, traceFlags);
    }

    private static bool IsValidVersion(string version)
    {
        if (version.Length != 2)
            return false;

        if (version != ValidVersion)
            return false;

        return IsHexOnly(version);
    }

    private static bool IsValidTraceId(string traceId)
    {
        if (traceId.Length != TraceIdLength)
            return false;

        if (!IsHexOnly(traceId))
            return false;

        if (traceId.All(c => c == '0'))
            return false;

        return true;
    }

    private static bool IsValidParentId(string parentId)
    {
        if (parentId.Length != ParentIdLength)
            return false;

        if (!IsHexOnly(parentId))
            return false;

        if (parentId.All(c => c == '0'))
            return false;

        return true;
    }

    private static bool IsHexOnly(string value)
    {
        return value.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'));
    }

    private static (string TraceId, string ParentId, string TraceFlags) RestartTrace()
    {
        string newTraceId = GenerateTraceId();
        string newParentId = "0000000000000000";
        string newTraceFlags = "00";
        return (newTraceId, newParentId, newTraceFlags);
    }

    private static string GenerateTraceId()
    {
        byte[] bytes = new byte[16];
        Random.Shared.NextBytes(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string GenerateSpanId()
    {
        byte[] bytes = new byte[8];
        Random.Shared.NextBytes(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}