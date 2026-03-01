using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.ApiKeys;

namespace HoroscopeApi.Application.Features.ApiKeys.CreateApiKeys;

public sealed record CreateApiKeyCommand(
    ApiKeyType Type,
    ApiKeyRateLimitType RateLimitType,
    int? RateLimitCount = null,
    int? RateLimit = null,
    List<string>? Scopes = null);

public sealed record CreateApiKeysCommands(List<CreateApiKeyCommand> Commands)
    : ICommand<PagedResponse<CreateApiKeyData>>;