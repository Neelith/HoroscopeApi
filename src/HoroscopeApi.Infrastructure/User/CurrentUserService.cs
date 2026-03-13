using System.Security.Claims;
using HoroscopeApi.Application.Infrastructure.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HoroscopeApi.Infrastructure.User;

internal class CurrentUserService(
    ILogger<CurrentUserService> logger,
    IHttpContextAccessor httpContextAccessor)
    : ICurrentUserService
{
    public string GetCurrentUserId()
    {
        ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;

        if (user is not { Identity.IsAuthenticated: true })
        {
            logger.LogError("Attempted to get current user ID, but user is not authenticated.");
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        string? userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogError("Current user ID is null or empty.");
            throw new InvalidOperationException("Current user ID is null or empty.");
        }

        logger.LogDebug("Current user ID retrieved: {UserId}", userId);

        return userId;
    }

    public bool IsCurrentUserAuthenticated()
    {
        return httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }

    public CurrentUser GetCurrentUser()
    {
        string userIdString = GetCurrentUserId();

        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            logger.LogError("Current user ID '{UserId}' is not a valid GUID.", userIdString);
            throw new InvalidOperationException("Current user ID is not a valid GUID.");
        }

        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        string? name = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        string? email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        logger.LogDebug("Current user retrieved: {UserId}", userId);

        return new CurrentUser
        {
            Id = userId,
            Name = name,
            Email = email
        };
    }
}