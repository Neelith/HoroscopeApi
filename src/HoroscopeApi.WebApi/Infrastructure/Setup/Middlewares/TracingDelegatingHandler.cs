using System.Diagnostics;
using HoroscopeApi.WebApi.Constants;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Middlewares;

public class TracingDelegatingHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Activity? activity = Activity.Current;

        if (activity != null)
        {
            string traceId = activity.TraceId.ToString();
            string spanId = activity.SpanId.ToString();
            string traceFlags = activity.Recorded ? "01" : "00";

            request.Headers.TryAddWithoutValidation(Headers.TraceParent, $"00-{traceId}-{spanId}-{traceFlags}");
        }

        return base.SendAsync(request, cancellationToken);
    }
}