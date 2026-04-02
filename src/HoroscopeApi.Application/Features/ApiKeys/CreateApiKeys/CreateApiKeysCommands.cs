using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.ApiKeys;

namespace HoroscopeApi.Application.Features.ApiKeys.CreateApiKeys;

public sealed record CreateApiKeyCommand(
    string Name,
    ApiKeyType Type,
    ApiKeyRateLimitType RateLimitType,
    int? RateLimit = null,
    DateTime? ExpiresAtUtc = null,
    List<string>? Scopes = null);

public sealed record CreateApiKeysCommands(List<CreateApiKeyCommand> Commands)
    : ICommand<PagedResponse<CreateApiKeyData>>;