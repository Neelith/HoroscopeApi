using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.WebApi.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Authorization.ApiKeyPolicy;

public class ApiKeyAuthorizationHandler(
    IHttpContextAccessor accessor,
    IApiKeyService apiKeyService)
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
            context.Fail();
            return;
        }

        string? providedApiKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrEmpty(providedApiKey))
        {
            context.Fail();
            return;
        }

        Result<ApiKeyValidation> apiKeyValidationResult =
            await apiKeyService.ValidateKeyAsync(providedApiKey, CancellationToken.None);

        if (apiKeyValidationResult.IsFailure)
        {
            context.Fail();
            return;
        }

        ApiKeyValidation apiKeyValidation = apiKeyValidationResult.Value!;

        if (apiKeyValidation.IsValid)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}