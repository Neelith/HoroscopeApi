using System.Diagnostics;

namespace HoroscopeApi.Infrastructure.Middlewares;

public class TracingDelegatingHandler : DelegatingHandler
{
    private const string TraceParentHeader = "traceparent";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Activity? activity = Activity.Current;

        if (activity != null)
        {
            string traceId = activity.TraceId.ToString();
            string spanId = activity.SpanId.ToString();
            string traceFlags = activity.Recorded ? "01" : "00";

            request.Headers.TryAddWithoutValidation(TraceParentHeader, $"00-{traceId}-{spanId}-{traceFlags}");
        }

        return base.SendAsync(request, cancellationToken);
    }
}