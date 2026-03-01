namespace HoroscopeApi.Application.Services.ApiKey;

public interface IApiKeyService
{
    Task<Result<ApiKeyValidation>> ValidateKeyAsync(string apiKey, CancellationToken cancellationToken);
    string GeneratePlainTextKey(int keySize = 56);
    string GenerateSalt(int saltSize = 8);
    string ComputeHash(string plainTextKey, string salt);
}