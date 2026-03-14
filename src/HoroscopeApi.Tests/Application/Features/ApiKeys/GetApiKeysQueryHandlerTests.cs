using HoroscopeApi.Application.Features.ApiKeys.GetApiKeys;
using HoroscopeApi.Application.Infrastructure.User;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.ApiKeys.Repositories;

namespace HoroscopeApi.Tests.Application.Features.ApiKeys;

public sealed class GetApiKeysQueryHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IApiKeyRepository> _repositoryMock = new();

    public GetApiKeysQueryHandlerTests()
    {
        _currentUserServiceMock
            .Setup(s => s.GetCurrentUser())
            .Returns(new CurrentUser { Id = Guid.NewGuid() });
    }

    private GetApiKeysQueryHandler CreateHandler()
    {
        return new GetApiKeysQueryHandler(_repositoryMock.Object, _currentUserServiceMock.Object);
    }

    private static ApiKey BuildApiKey(int id = 1)
    {
        return new ApiKey
        {
            Id = id,
            OwnerId = Guid.NewGuid(),
            Prefix = "abcdef12",
            Hash = "hash==",
            Salt = "salt==",
            Algorithm = "HMAC-SHA256",
            Type = ApiKeyType.Permanent,
            RateLimitType = ApiKeyRateLimitType.None
        };
    }

    [Fact]
    public async Task Handle_WithNoFilters_ReturnsAllKeys()
    {
        List<ApiKey> apiKeys = new() { BuildApiKey(), BuildApiKey(2) };
        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiKeys);

        GetApiKeysQueryHandler handler = CreateHandler();
        Result<PagedResponse<ApiKeyData>> result = await handler.Handle(new GetApiKeysQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Data.Items.Count());
    }

    [Fact]
    public async Task Handle_WithIdsFilter_ParsesAndPassesIds()
    {
        List<ApiKey> apiKeys = new() { BuildApiKey() };
        GetApiKeysByFilterRepositoryQuery? capturedQuery = null;

        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .Callback<GetApiKeysByFilterRepositoryQuery, CancellationToken>((q, _) => capturedQuery = q)
            .ReturnsAsync(apiKeys);

        GetApiKeysQueryHandler handler = CreateHandler();
        Result<PagedResponse<ApiKeyData>> result =
            await handler.Handle(new GetApiKeysQuery(Ids: "1,2,3"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedQuery);
        Assert.Equal([1, 2, 3], capturedQuery!.Ids);
    }

    [Fact]
    public async Task Handle_WithNullIds_PassesNullIdsToRepository()
    {
        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        GetApiKeysByFilterRepositoryQuery? capturedQuery = null;
        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .Callback<GetApiKeysByFilterRepositoryQuery, CancellationToken>((q, _) => capturedQuery = q)
            .ReturnsAsync([]);

        GetApiKeysQueryHandler handler = CreateHandler();
        await handler.Handle(new GetApiKeysQuery(), CancellationToken.None);

        Assert.Null(capturedQuery!.Ids);
    }

    [Fact]
    public async Task Handle_WithTypeFilter_PassesTypeToRepository()
    {
        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        GetApiKeysByFilterRepositoryQuery? capturedQuery = null;
        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .Callback<GetApiKeysByFilterRepositoryQuery, CancellationToken>((q, _) => capturedQuery = q)
            .ReturnsAsync([]);

        GetApiKeysQueryHandler handler = CreateHandler();
        await handler.Handle(new GetApiKeysQuery(ApiKeyType.Permanent), CancellationToken.None);

        Assert.Equal(ApiKeyType.Permanent, capturedQuery!.Type);
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyPagedResponse()
    {
        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        GetApiKeysQueryHandler handler = CreateHandler();
        Result<PagedResponse<ApiKeyData>> result = await handler.Handle(new GetApiKeysQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Data.Items);
    }

    [Fact]
    public async Task Handle_MapsApiKeyDataCorrectly()
    {
        ApiKey apiKey = new()
        {
            Id = 5,
            OwnerId = Guid.NewGuid(),
            Prefix = "mypref12",
            Hash = "hash==",
            Salt = "salt==",
            Algorithm = "HMAC-SHA256",
            Type = ApiKeyType.Temporary,
            RateLimitType = ApiKeyRateLimitType.PerDay,
            RateLimitCount = 500,
            Scopes = [new ApiKeyScope { Id = 1, ApiKeyId = 5, Name = "read" }]
        };

        _repositoryMock.Setup(r =>
                r.GetByFilterAsync(It.IsAny<GetApiKeysByFilterRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([apiKey]);

        GetApiKeysQueryHandler handler = CreateHandler();
        Result<PagedResponse<ApiKeyData>> result = await handler.Handle(new GetApiKeysQuery(), CancellationToken.None);

        ApiKeyData item = result.Value!.Data.Items.First();
        Assert.Equal(5, item.Id);
        Assert.Equal("mypref12", item.Prefix);
        Assert.Equal("Temporary", item.Type);
        Assert.Equal("PerDay", item.RateLimitType);
        Assert.Equal(500, item.RateLimitCount);
        Assert.Contains("read", item.Scopes);
    }
}