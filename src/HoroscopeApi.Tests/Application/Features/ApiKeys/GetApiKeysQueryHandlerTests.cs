using HoroscopeApi.Application.Features.ApiKeys.GetApiKeys;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.ApiKeys.Repositories;

namespace HoroscopeApi.Tests.Application.Features.ApiKeys;

public sealed class GetApiKeysQueryHandlerTests
{
    private readonly Mock<IApiKeyRepository> _repositoryMock = new();

    private GetApiKeysQueryHandler CreateHandler() => new(_repositoryMock.Object);

    private static ApiKey BuildApiKey(int id = 1) => new()
    {
        Id = id,
        Prefix = "abcdef12",
        Hash = "hash==",
        Salt = "salt==",
        Algorithm = "HMAC-SHA256",
        Type = ApiKeyType.Permanent,
        RateLimitType = ApiKeyRateLimitType.None
    };

    [Fact]
    public async Task Handle_WithNoFilters_ReturnsAllKeys()
    {
        var apiKeys = new List<ApiKey> { BuildApiKey(1), BuildApiKey(2) };
        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiKeys);

        var handler = CreateHandler();
        var result = await handler.Handle(new GetApiKeysQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Data.Items.Count());
    }

    [Fact]
    public async Task Handle_WithIdsFilter_ParsesAndPassesIds()
    {
        var apiKeys = new List<ApiKey> { BuildApiKey(1) };
        GetApiKeysByFilterRepositoryQuery? capturedQuery = null;

        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .Callback<GetApiKeysByFilterRepositoryQuery, CancellationToken>((q, _) => capturedQuery = q)
            .ReturnsAsync(apiKeys);

        var handler = CreateHandler();
        var result = await handler.Handle(new GetApiKeysQuery(Ids: "1,2,3"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedQuery);
        Assert.Equal([1, 2, 3], capturedQuery!.Ids);
    }

    [Fact]
    public async Task Handle_WithNullIds_PassesNullIdsToRepository()
    {
        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        GetApiKeysByFilterRepositoryQuery? capturedQuery = null;
        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .Callback<GetApiKeysByFilterRepositoryQuery, CancellationToken>((q, _) => capturedQuery = q)
            .ReturnsAsync([]);

        var handler = CreateHandler();
        await handler.Handle(new GetApiKeysQuery(), CancellationToken.None);

        Assert.Null(capturedQuery!.Ids);
    }

    [Fact]
    public async Task Handle_WithTypeFilter_PassesTypeToRepository()
    {
        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        GetApiKeysByFilterRepositoryQuery? capturedQuery = null;
        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .Callback<GetApiKeysByFilterRepositoryQuery, CancellationToken>((q, _) => capturedQuery = q)
            .ReturnsAsync([]);

        var handler = CreateHandler();
        await handler.Handle(new GetApiKeysQuery(Type: ApiKeyType.Permanent), CancellationToken.None);

        Assert.Equal(ApiKeyType.Permanent, capturedQuery!.Type);
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyPagedResponse()
    {
        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = CreateHandler();
        var result = await handler.Handle(new GetApiKeysQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Data.Items);
    }

    [Fact]
    public async Task Handle_MapsApiKeyDataCorrectly()
    {
        var apiKey = new ApiKey
        {
            Id = 5,
            Prefix = "mypref12",
            Hash = "hash==",
            Salt = "salt==",
            Algorithm = "HMAC-SHA256",
            Type = ApiKeyType.Temporary,
            RateLimitType = ApiKeyRateLimitType.PerDay,
            RateLimitCount = 500,
            Scopes = [new ApiKeyScope { Id = 1, ApiKeyId = 5, Name = "read" }]
        };

        _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([apiKey]);

        var handler = CreateHandler();
        var result = await handler.Handle(new GetApiKeysQuery(), CancellationToken.None);

        var item = result.Value!.Data.Items.First();
        Assert.Equal(5, item.Id);
        Assert.Equal("mypref12", item.Prefix);
        Assert.Equal("Temporary", item.Type);
        Assert.Equal("PerDay", item.RateLimitType);
        Assert.Equal(500, item.RateLimitCount);
        Assert.Contains("read", item.Scopes);
    }
}
