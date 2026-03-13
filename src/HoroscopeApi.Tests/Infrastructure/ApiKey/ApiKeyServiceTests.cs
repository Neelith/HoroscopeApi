using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Domain.ApiKeys.Repositories;
using HoroscopeApi.Infrastructure.ApiKey;
using HoroscopeApi.WebApi.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using DomainApiKey = HoroscopeApi.Domain.ApiKeys.ApiKey;

namespace HoroscopeApi.Tests.Infrastructure.ApiKey;

public sealed class ApiKeyServiceTests
{
    private readonly Mock<IApiKeyRepository> _repositoryMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();

    private ApiKeyService CreateService(int prefixLength = 8, string secret = "super-secret-key") =>
        new(Options.Create(new ApiKeySettings { Secret = secret, PrefixLenght = prefixLength }),
            _repositoryMock.Object, _dateTimeProviderMock.Object);

    [Fact]
    public void GeneratePlainTextKey_ReturnsHexString()
    {
        var service = CreateService();
        var key = service.GeneratePlainTextKey();

        Assert.NotNull(key);
        Assert.True(key.Length == 112); // 56 bytes * 2 hex chars
        Assert.Matches("^[0-9a-f]+$", key);
    }

    [Fact]
    public void GeneratePlainTextKey_ReturnsDifferentKeysEachTime()
    {
        var service = CreateService();
        var key1 = service.GeneratePlainTextKey();
        var key2 = service.GeneratePlainTextKey();

        Assert.NotEqual(key1, key2);
    }

    [Fact]
    public void GenerateSalt_ReturnsBase64String()
    {
        var service = CreateService();
        var salt = service.GenerateSalt();

        Assert.NotNull(salt);
        var bytes = Convert.FromBase64String(salt);
        Assert.Equal(8, bytes.Length);
    }

    [Fact]
    public void GenerateSalt_ReturnsDifferentSaltsEachTime()
    {
        var service = CreateService();
        var salt1 = service.GenerateSalt();
        var salt2 = service.GenerateSalt();

        Assert.NotEqual(salt1, salt2);
    }

    [Fact]
    public void ComputeHash_IsDeterministic()
    {
        var service = CreateService();
        var salt = service.GenerateSalt();
        var plainText = service.GeneratePlainTextKey();

        var hash1 = service.ComputeHash(plainText, salt);
        var hash2 = service.ComputeHash(plainText, salt);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeHash_DifferentSaltsProduceDifferentHashes()
    {
        var service = CreateService();
        var plainText = service.GeneratePlainTextKey();
        var salt1 = service.GenerateSalt();
        var salt2 = service.GenerateSalt();

        var hash1 = service.ComputeHash(plainText, salt1);
        var hash2 = service.ComputeHash(plainText, salt2);

        Assert.NotEqual(hash1, hash2);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ValidateKeyAsync_WithEmptyKey_ReturnsFalseIsValid(string apiKey)
    {
        var service = CreateService();

        var result = await service.ValidateKeyAsync(apiKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithKeyTooShort_ReturnsFalseIsValid()
    {
        var service = CreateService(prefixLength: 8);

        var result = await service.ValidateKeyAsync("short", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithNoMatchingKeys_ReturnsFalseIsValid()
    {
        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(DateTime.UtcNow);

        var service = CreateService();
        var plainKey = service.GeneratePlainTextKey();

        var result = await service.ValidateKeyAsync(plainKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithValidPermanentKey_ReturnsTrueIsValid()
    {
        var service = CreateService(prefixLength: 8);
        var now = DateTime.UtcNow;
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);

        var plainKey = service.GeneratePlainTextKey();
        var salt = service.GenerateSalt();
        var hash = service.ComputeHash(plainKey, salt);
        var prefix = plainKey[..8];

        var apiKey = new DomainApiKey
        {
            Prefix = prefix,
            Hash = hash,
            Salt = salt,
            Algorithm = "HMAC-SHA256",
            Type = ApiKeyType.Permanent,
            RateLimitType = ApiKeyRateLimitType.None
        };

        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([apiKey]);

        var result = await service.ValidateKeyAsync(plainKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithValidTemporaryKeyNotExpired_ReturnsTrueIsValid()
    {
        var service = CreateService(prefixLength: 8);
        var now = DateTime.UtcNow;
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);

        var plainKey = service.GeneratePlainTextKey();
        var salt = service.GenerateSalt();
        var hash = service.ComputeHash(plainKey, salt);
        var prefix = plainKey[..8];

        var apiKey = new DomainApiKey
        {
            Prefix = prefix,
            Hash = hash,
            Salt = salt,
            Algorithm = "HMAC-SHA256",
            Type = ApiKeyType.Temporary,
            RateLimitType = ApiKeyRateLimitType.None,
            ExpiresAtUtc = now.AddDays(1)
        };

        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([apiKey]);

        var result = await service.ValidateKeyAsync(plainKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithExpiredTemporaryKey_ReturnsFalseIsValid()
    {
        var service = CreateService(prefixLength: 8);
        var now = DateTime.UtcNow;
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);

        var plainKey = service.GeneratePlainTextKey();
        var salt = service.GenerateSalt();
        var hash = service.ComputeHash(plainKey, salt);
        var prefix = plainKey[..8];

        var apiKey = new DomainApiKey
        {
            Prefix = prefix,
            Hash = hash,
            Salt = salt,
            Algorithm = "HMAC-SHA256",
            Type = ApiKeyType.Temporary,
            RateLimitType = ApiKeyRateLimitType.None,
            ExpiresAtUtc = now.AddDays(-1)
        };

        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([apiKey]);

        var result = await service.ValidateKeyAsync(plainKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value!.IsValid);
    }

    [Fact]
    public async Task ValidateKeyAsync_WithWrongHash_ReturnsFalseIsValid()
    {
        var service = CreateService(prefixLength: 8);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(DateTime.UtcNow);

        var plainKey = service.GeneratePlainTextKey();
        var salt = service.GenerateSalt();
        var prefix = plainKey[..8];

        var apiKey = new DomainApiKey
        {
            Prefix = prefix,
            Hash = "wronghash==",
            Salt = salt,
            Algorithm = "HMAC-SHA256",
            Type = ApiKeyType.Permanent,
            RateLimitType = ApiKeyRateLimitType.None
        };

        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([apiKey]);

        var result = await service.ValidateKeyAsync(plainKey, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value!.IsValid);
    }
}
