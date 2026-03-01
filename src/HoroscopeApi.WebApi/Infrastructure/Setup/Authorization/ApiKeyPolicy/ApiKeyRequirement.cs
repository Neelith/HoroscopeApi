using Microsoft.AspNetCore.Authorization;

namespace HoroscopeApi.WebApi.Infrastructure.Setup.Authorization.ApiKeyPolicy;

public record ApiKeyRequirement : IAuthorizationRequirement;