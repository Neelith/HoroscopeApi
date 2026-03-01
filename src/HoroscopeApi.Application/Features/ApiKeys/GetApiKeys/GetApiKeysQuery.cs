using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.ApiKeys;

namespace HoroscopeApi.Application.Features.ApiKeys.GetApiKeys;

public sealed record GetApiKeysQuery(
    ApiKeyType? Type = null,
    ApiKeyRateLimitType? RateLimitType = null,
    string? Ids = null)
    : IQuery<PagedResponse<ApiKeyData>>;