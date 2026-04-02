using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.Domain.ApiKeys;
using HoroscopeApi.WebApi.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Authorization.ApiKeyPolicy;

public class ApiKeyAuthorizationHandler(
    IHttpContextAccessor accessor,
    IApiKeyService apiKeyService,
    IApiKeyRateLimiter apiKeyRateLimiter)
    : AuthorizationHandler<ApiKeyRequirement>
{
    private const string RateLimitLimitHeader = "X-RateLimit-Limit";
    private const string RateLimitRemainingHeader = "X-RateLimit-Remaining";
    private const string RateLimitResetHeader = "X-RateLimit-Reset";
    private const string RetryAfterHeader = "Retry-After";

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ApiKeyRequirement requirement)
    {
        HttpContext? httpContext = accessor.HttpContext;
        if (httpContext is null)
        {
            context.Fail();
            return;
        }

        if (!httpContext.Request.Headers.TryGetValue(Headers.ApiKey, out StringValues apiKeyHeaderValues))
        {
            context.Fail();
            return;
        }

        string? providedApiKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrEmpty(providedApiKey))
        {
            context.Fail();
            return;
        }

        // Validate the API key
        Result<ApiKeyValidation> apiKeyValidationResult =
            await apiKeyService.ValidateKeyAsync(providedApiKey, CancellationToken.None);

        if (apiKeyValidationResult.IsFailure)
        {
            context.Fail();
            return;
        }

        ApiKeyValidation apiKeyValidation = apiKeyValidationResult.Value!;

        if (apiKeyValidation.IsNotValid)
        {
            context.Fail();
            return;
        }

        // Check scopes
        if (!HasRequiredScope(httpContext, apiKeyValidation))
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Fail();
            return;
        }

        // Check rate limiting
        if (apiKeyValidation.RateLimitType is not null and not ApiKeyRateLimitType.None
            && apiKeyValidation.RateLimit is not null)
        {
            ApiKeyRateLimitResult rateLimitResult = await apiKeyRateLimiter.CheckRateLimitAsync(
                apiKeyValidation.ApiKeyId!.Value,
                apiKeyValidation.RateLimitType.Value,
                apiKeyValidation.RateLimit.Value,
                CancellationToken.None);

            SetRateLimitHeaders(httpContext, rateLimitResult);

            if (rateLimitResult.IsExceeded)
            {
                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Fail();
                return;
            }
        }

        context.Succeed(requirement);
    }

    private static bool HasRequiredScope(HttpContext httpContext, ApiKeyValidation validation)
    {
        ApiKeyScopeMetadata? scopeMetadata = httpContext.GetEndpoint()?.Metadata.GetMetadata<ApiKeyScopeMetadata>();

        // No scope metadata on the endpoint — allow (not scope-protected)
        if (scopeMetadata is null)
        {
            return true;
        }

        // Key has no scopes defined — deny access
        if (validation.Scopes is null || validation.Scopes.Count == 0)
        {
            return false;
        }

        return validation.Scopes.Contains(scopeMetadata.Scope, StringComparer.OrdinalIgnoreCase);
    }

    private static void SetRateLimitHeaders(HttpContext httpContext, ApiKeyRateLimitResult rateLimitResult)
    {
        httpContext.Response.Headers[RateLimitLimitHeader] = rateLimitResult.Limit.ToString();
        httpContext.Response.Headers[RateLimitRemainingHeader] = rateLimitResult.Remaining.ToString();
        httpContext.Response.Headers[RateLimitResetHeader] = rateLimitResult.ResetAtUtc.ToString("o");

        if (rateLimitResult.IsExceeded)
        {
            long retryAfterSeconds = (long)Math.Ceiling(
                (rateLimitResult.ResetAtUtc - DateTime.UtcNow).TotalSeconds);
            httpContext.Response.Headers[RetryAfterHeader] = Math.Max(1, retryAfterSeconds).ToString();
        }
    }
}