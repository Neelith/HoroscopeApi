using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Application.Settings;
using HoroscopeApi.Domain.ApiKeys.Repositories;
using HoroscopeApi.Infrastructure.ApiKey;
using Microsoft.Extensions.Options;
using DomainApiKey = HoroscopeApi.Domain.ApiKeys.ApiKey;

namespace HoroscopeApi.Tests.Infrastructure.ApiKey;

public sealed class ApiKeyServiceTests
{
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly Mock<IApiKeyRepository> _repositoryMock = new();

    private ApiKeyService CreateService(int prefixLength = 8, string secret = "super-secret-key")
    {
        return new ApiKeyService(Options.Create(new ApiKeySettings { Secret = secret, PrefixLenght = prefixLength }),
            _repositoryMock.Object, _dateTimeProviderMock.Object);
    }

    [Fact]
    public void GeneratePlainTextKey_ReturnsHexString()
    {
        ApiKeyService service = CreateService();
        string key = service.GeneratePlainTextKey();

        Assert.NotNull(key);
        Assert.True(key.Length == 112); // 56 bytes * 2 hex chars
        Assert.Matches("^[0-9a-f]+$", key);
    }

    [Fact]
    public void GeneratePlainTextKey_ReturnsDifferentKeysEachTime()
    {
        ApiKeyService service = CreateService();
        string key1 = service.GeneratePlainTextKey();
        string key2 = service.GeneratePlainTextKey();

        Assert.NotEqual(key1, key2);
    }

    [Fact]
    public void GenerateSalt_ReturnsBase64String()
    {
        ApiKeyService service = CreateService();
        string salt = service.GenerateSalt();

        Assert.NotNull(salt);
        byte[] bytes = Convert.FromBase64String(salt);
        Assert.Equal(8, bytes.Length);
    }

    [Fact]
    public void GenerateSalt_ReturnsDifferentSaltsEachTime()
    {
        ApiKeyService service = CreateService();
        string salt1 = service.GenerateSalt();
        string salt2 = service.GenerateSalt();

        Assert.NotEqual(salt1, salt2);
    }

    [Fact]
    public void ComputeHash_IsDeterministic()
    {
        ApiKeyService service = CreateService();
        string salt = service.GenerateSalt();
        string plainText = service.GeneratePlainTextKey();

        string hash1 = service.ComputeHash(plainText, salt);
        string hash2 = service.ComputeHash(plainText, salt);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeHash_DifferentSaltsProduceDifferentHashes()
    {
        ApiKeyService service = CreateService();
        string plainText = service.GeneratePlainTextKey();
        string salt1 = service.GenerateSalt();
        string salt2 = service.GenerateSalt();

        string hash1 = service.ComputeHash(plainText, salt1);
        string hash2 = service.ComputeHash(plainText, salt2);

        Assert.NotEqual(hash1, hash2);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ValidateKeyAsync_WithEmptyKey_ReturnsFalseIsValid(string apiKey)
    {
        ApiKeyService service = CreateService();

        Result<ApiKeyValidation> result = await service.ValidateKeyAsync(apiKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithKeyTooShort_ReturnsFalseIsValid()
    {
        ApiKeyService service = CreateService();

        Result<ApiKeyValidation> result = await service.ValidateKeyAsync("short", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithNoMatchingKeys_ReturnsFalseIsValid()
    {
        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(DateTime.UtcNow);

        ApiKeyService service = CreateService();
        string plainKey = service.GeneratePlainTextKey();

        Result<ApiKeyValidation> result = await service.ValidateKeyAsync(plainKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithValidPermanentKey_ReturnsTrueIsValid()
    {
        ApiKeyService service = CreateService();
        DateTime now = DateTime.UtcNow;
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);

        string plainKey = service.GeneratePlainTextKey();
        string salt = service.GenerateSalt();
        string hash = service.ComputeHash(plainKey, salt);
        string prefix = plainKey[..8];

        DomainApiKey apiKey = new()
        {
            OwnerId = Guid.NewGuid(),
            Prefix = prefix,
            Hash = hash,
            Salt = salt,
            Algorithm = "HMAC-SHA256",
            Type = ApiKeyType.Permanent,
            RateLimitType = ApiKeyRateLimitType.None
        };

        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([apiKey]);

        Result<ApiKeyValidation> result = await service.ValidateKeyAsync(plainKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithValidTemporaryKeyNotExpired_ReturnsTrueIsValid()
    {
        ApiKeyService service = CreateService();
        DateTime now = DateTime.UtcNow;
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);

        string plainKey = service.GeneratePlainTextKey();
        string salt = service.GenerateSalt();
        string hash = service.ComputeHash(plainKey, salt);
        string prefix = plainKey[..8];

        DomainApiKey apiKey = new()
        {
            OwnerId = Guid.NewGuid(),
            Prefix = prefix,
            Hash = hash,
            Salt = salt,
            Algorithm = "HMAC-SHA256",
            Type = ApiKeyType.Temporary,
            RateLimitType = ApiKeyRateLimitType.None,
            ExpiresAtUtc = now.AddDays(1)
        };

        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([apiKey]);

        Result<ApiKeyValidation> result = await service.ValidateKeyAsync(plainKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithExpiredTemporaryKey_ReturnsFalseIsValid()
    {
        ApiKeyService service = CreateService();
        DateTime now = DateTime.UtcNow;
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);

        string plainKey = service.GeneratePlainTextKey();
        string salt = service.GenerateSalt();
        string hash = service.ComputeHash(plainKey, salt);
        string prefix = plainKey[..8];

        DomainApiKey apiKey = new()
        {
            OwnerId = Guid.NewGuid(),
            Prefix = prefix,
            Hash = hash,
            Salt = salt,
            Algorithm = "HMAC-SHA256",
            Type = ApiKeyType.Temporary,
            RateLimitType = ApiKeyRateLimitType.None,
            ExpiresAtUtc = now.AddDays(-1)
        };

        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([apiKey]);

        Result<ApiKeyValidation> result = await service.ValidateKeyAsync(plainKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithWrongHash_ReturnsFalseIsValid()
    {
        ApiKeyService service = CreateService();
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(DateTime.UtcNow);

        string plainKey = service.GeneratePlainTextKey();
        string salt = service.GenerateSalt();
        string prefix = plainKey[..8];

        DomainApiKey apiKey = new()
        {
            OwnerId = Guid.NewGuid(),
            Prefix = prefix,
            Hash = "wronghash==",
            Salt = salt,
            Algorithm = "HMAC-SHA256",
            Type = ApiKeyType.Permanent,
            RateLimitType = ApiKeyRateLimitType.None
        };

        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([apiKey]);

        Result<ApiKeyValidation> result = await service.ValidateKeyAsync(plainKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value!.IsValid);
    }
}