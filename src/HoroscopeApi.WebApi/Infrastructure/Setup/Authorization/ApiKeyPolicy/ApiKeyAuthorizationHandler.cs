using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.Domain.ApiKeys;
using HoroscopeApi.WebApi.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Authorization.ApiKeyPolicy;

public class ApiKeyAuthorizationHandler(
    IHttpContextAccessor accessor,
    IApiKeyService apiKeyService,
    IApiKeyRateLimiter apiKeyRateLimiter,
    ILogger<ApiKeyAuthorizationHandler> logger)
    : AuthorizationHandler<ApiKeyRequirement>
{
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
            logger.LogWarning("API key header '{Header}' is missing from the request", Headers.ApiKey);
            context.Fail();
            return;
        }

        string? providedApiKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrEmpty(providedApiKey))
        {
            logger.LogWarning("API key header '{Header}' is present but empty", Headers.ApiKey);
            context.Fail();
            return;
        }

        // Validate the API key
        Result<ApiKeyValidation> apiKeyValidationResult =
            await apiKeyService.ValidateKeyAsync(providedApiKey, CancellationToken.None);

        if (apiKeyValidationResult.IsFailure)
        {
            logger.LogWarning("API key validation failed for key with prefix '{Prefix}'",
                providedApiKey.Length >= 8 ? providedApiKey[..8] : providedApiKey);
            context.Fail();
            return;
        }

        ApiKeyValidation apiKeyValidation = apiKeyValidationResult.Value!;

        if (apiKeyValidation.IsNotValid)
        {
            logger.LogWarning("API key is invalid or expired for key with prefix '{Prefix}'",
                providedApiKey.Length >= 8 ? providedApiKey[..8] : providedApiKey);
            context.Fail();
            return;
        }

        // Check scopes
        if (!HasRequiredScope(httpContext, apiKeyValidation))
        {
            var scopeMetadata =
                httpContext.GetEndpoint()?.Metadata.GetMetadata<ApiKeyScopeMetadata>();

            logger.LogWarning(
                "API key {ApiKeyId} lacks required scope '{RequiredScope}'",
                apiKeyValidation.ApiKeyId, scopeMetadata?.Scope);

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
                logger.LogWarning(
                    "Rate limit exceeded for API key {ApiKeyId}. Limit: {Limit}, Remaining: {Remaining}, Resets at: {ResetAtUtc}",
                    apiKeyValidation.ApiKeyId, rateLimitResult.Limit, rateLimitResult.Remaining,
                    rateLimitResult.ResetAtUtc.ToString("o"));

                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Fail();
                return;
            }
        }

        logger.LogDebug("API key {ApiKeyId} authorized successfully", apiKeyValidation.ApiKeyId);
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
        httpContext.Response.Headers[Headers.RateLimitLimit] = rateLimitResult.Limit.ToString();
        httpContext.Response.Headers[Headers.RateLimitRemaining] = rateLimitResult.Remaining.ToString();
        httpContext.Response.Headers[Headers.RateLimitReset] = rateLimitResult.ResetAtUtc.ToString("o");

        if (rateLimitResult.IsExceeded)
        {
            long retryAfterSeconds = (long)Math.Ceiling(
                (rateLimitResult.ResetAtUtc - DateTime.UtcNow).TotalSeconds);
            httpContext.Response.Headers[Headers.RetryAfter] = Math.Max(1, retryAfterSeconds).ToString();
        }
    }
}