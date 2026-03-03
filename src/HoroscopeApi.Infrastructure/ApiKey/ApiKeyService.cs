using System.Security.Cryptography;
using System.Text;
using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Domain.ApiKeys;
using HoroscopeApi.Domain.ApiKeys.Repositories;
using HoroscopeApi.WebApi.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace HoroscopeApi.Infrastructure.ApiKey;

internal class ApiKeyService(
    IOptions<ApiKeySettings> apiKeySettingsOptions,
    IApiKeyRepository apiKeyRepository,
    IDateTimeProvider dateTimeProvider) : IApiKeyService
{
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

            if (
                (key.Type == ApiKeyType.Permanent ||
                 (key is { Type: ApiKeyType.Temporary, ExpiresAtUtc: not null } &&
                  key.ExpiresAtUtc.Value >= dateTimeProvider.UtcNow)) &&
                keyHash.Equals(key.Hash, StringComparison.InvariantCulture))
            {
                return Result.Ok(new ApiKeyValidation(true));
            }
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
}