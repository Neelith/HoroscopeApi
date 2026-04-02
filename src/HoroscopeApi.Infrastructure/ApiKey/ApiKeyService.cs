using System.Security.Cryptography;
using System.Text;
using HoroscopeApi.Application.Infrastructure.Caching;
using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Application.Settings;
using HoroscopeApi.Domain.ApiKeys;
using HoroscopeApi.Domain.ApiKeys.Repositories;
using Microsoft.Extensions.Options;

namespace HoroscopeApi.Infrastructure.ApiKey;

internal class ApiKeyService(
    IOptions<ApiKeySettings> apiKeySettingsOptions,
    IApiKeyRepository apiKeyRepository,
    IDateTimeProvider dateTimeProvider,
    IRedisCache redisCache) : IApiKeyService
{
    private const string CacheKeyPrefix = "apikey:validation:";
    private static readonly TimeSpan ValidationCacheTtl = TimeSpan.FromMinutes(1);

    private readonly ApiKeySettings _apiKeySettings = apiKeySettingsOptions.Value;

    public async Task<Result<ApiKeyValidation>> ValidateKeyAsync(string apiKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Result.Ok(new ApiKeyValidation(false));
        }

        if (apiKey.Length < _apiKeySettings.PrefixLenght)
        {
            return Result.Ok(new ApiKeyValidation(false));
        }

        // Check cache first using a hash of the API key as cache key
        string cacheKey = CacheKeyPrefix + ComputeCacheKey(apiKey);
        ApiKeyValidation? cached = await redisCache.GetAsync<ApiKeyValidation>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result.Ok(cached);
        }

        string prefix = apiKey[.._apiKeySettings.PrefixLenght];

        List<Domain.ApiKeys.ApiKey> apiKeys =
            await apiKeyRepository.GetByFilterAsync(
                new GetApiKeysByFilterRepositoryQuery(Prefixes: [prefix]),
                cancellationToken) ?? [];

        if (apiKeys.Count == 0)
        {
            return Result.Ok(new ApiKeyValidation(false));
        }

        foreach (Domain.ApiKeys.ApiKey key in apiKeys)
        {
            string keyHash = ComputeHash(apiKey, key.Salt);

            bool hashMatches = keyHash.Equals(key.Hash, StringComparison.InvariantCulture);
            if (!hashMatches)
            {
                continue;
            }

            bool isValid = key.Type == ApiKeyType.Permanent ||
                           (key is { Type: ApiKeyType.Temporary, ExpiresAtUtc: not null } &&
                            key.ExpiresAtUtc.Value >= dateTimeProvider.UtcNow);

            if (!isValid)
            {
                continue;
            }

            ApiKeyValidation validation = new(
                IsValid: true,
                ApiKeyId: key.Id,
                RateLimitType: key.RateLimitType,
                RateLimitCount: key.RateLimitCount,
                RateLimit: key.RateLimit,
                Scopes: key.Scopes.Select(s => s.Name).ToList());

            await redisCache.SetAsync(cacheKey, validation, ValidationCacheTtl, cancellationToken);

            return Result.Ok(validation);
        }

        return Result.Ok(new ApiKeyValidation(false));
    }

    public string GeneratePlainTextKey(int keySize = 56)
    {
        byte[] keyBytes = RandomNumberGenerator.GetBytes(keySize);
        return Convert.ToHexStringLower(keyBytes);
    }

    public string GenerateSalt(int saltSize = 8)
    {
        byte[] saltBytes = RandomNumberGenerator.GetBytes(saltSize);
        return Convert.ToBase64String(saltBytes);
    }

    public string ComputeHash(string plainTextKey, string salt)
    {
        string secretString = _apiKeySettings.Secret ?? throw new InvalidOperationException("No secret provided");
        byte[] saltBytes = Convert.FromBase64String(salt);
        byte[] secret = Encoding.UTF8.GetBytes(secretString);
        byte[] key = secret.Concat(saltBytes).ToArray();

        using HMACSHA256 hmac = new(key);

        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainTextKey));
        return Convert.ToBase64String(hashBytes);
    }

    private static string ComputeCacheKey(string apiKey)
    {
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToHexStringLower(hashBytes);
    }
}