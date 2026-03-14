using HoroscopeApi.Application.Features.ApiKeys.CreateApiKeys;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Application.Infrastructure.User;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.Domain.ApiKeys.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HoroscopeApi.Tests.Application.Features.ApiKeys;

public sealed class CreateApiKeysCommandHandlerTests
{
    private readonly Mock<IApiKeyService> _apiKeyServiceMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly ILogger<CreateApiKeysCommandHandler> _logger = NullLogger<CreateApiKeysCommandHandler>.Instance;
    private readonly Mock<IApiKeyRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public CreateApiKeysCommandHandlerTests()
    {
        _currentUserServiceMock
            .Setup(s => s.GetCurrentUser())
            .Returns(new CurrentUser { Id = Guid.NewGuid() });
    }

    private CreateApiKeysCommandHandler CreateHandler()
    {
        return new CreateApiKeysCommandHandler(_logger, _repositoryMock.Object, _unitOfWorkMock.Object,
            _apiKeyServiceMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommands_ReturnsPagedResponseWithCreatedKeys()
    {
        _apiKeyServiceMock.Setup(s => s.GeneratePlainTextKey(It.IsAny<int>()))
            .Returns("abcdef1234567890");
        _apiKeyServiceMock.Setup(s => s.GenerateSalt(It.IsAny<int>()))
            .Returns("dGVzdA==");
        _apiKeyServiceMock.Setup(s => s.ComputeHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("hashvalue==");

        _repositoryMock.Setup(r =>
                r.UpsertRangeAsync(It.IsAny<UpsertApiKeysRepositoryCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        CreateApiKeyCommand command = new(ApiKeyType.Permanent, ApiKeyRateLimitType.None);
        CreateApiKeysCommands commands = new([command]);
        CreateApiKeysCommandHandler handler = CreateHandler();

        Result<PagedResponse<CreateApiKeyData>> result = await handler.Handle(commands, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        List<CreateApiKeyData> items = result.Value!.Data.Items.ToList();
        Assert.Single(items);
        Assert.Equal("abcdef12", items[0].Prefix);
        Assert.Equal("abcdef1234567890", items[0].PlainTextKey);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_ReturnsAllCreatedKeys()
    {
        int callCount = 0;
        _apiKeyServiceMock.Setup(s => s.GeneratePlainTextKey(It.IsAny<int>()))
            .Returns(() => callCount++ == 0 ? "abcdef1234567890" : "1234567890abcdef");
        _apiKeyServiceMock.Setup(s => s.GenerateSalt(It.IsAny<int>()))
            .Returns("dGVzdA==");
        _apiKeyServiceMock.Setup(s => s.ComputeHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("hash==");

        _repositoryMock.Setup(r =>
                r.UpsertRangeAsync(It.IsAny<UpsertApiKeysRepositoryCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        CreateApiKeysCommands commands = new([
            new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.None),
            new CreateApiKeyCommand(ApiKeyType.Temporary, ApiKeyRateLimitType.PerMinute, 100, 60)
        ]);
        CreateApiKeysCommandHandler handler = CreateHandler();

        Result<PagedResponse<CreateApiKeyData>> result = await handler.Handle(commands, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Data.Items.Count());
    }

    [Fact]
    public async Task Handle_WhenSaveChangesThrows_ReturnsCreationFailedError()
    {
        _apiKeyServiceMock.Setup(s => s.GeneratePlainTextKey(It.IsAny<int>()))
            .Returns("abcdef1234567890");
        _apiKeyServiceMock.Setup(s => s.GenerateSalt(It.IsAny<int>()))
            .Returns("dGVzdA==");
        _apiKeyServiceMock.Setup(s => s.ComputeHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("hash==");

        _repositoryMock.Setup(r =>
                r.UpsertRangeAsync(It.IsAny<UpsertApiKeysRepositoryCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        CreateApiKeysCommands commands = new([
            new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.None)
        ]);
        CreateApiKeysCommandHandler handler = CreateHandler();

        Result<PagedResponse<CreateApiKeyData>> result = await handler.Handle(commands, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ApiKey.CreationFailed", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_WithScopes_IncludesScopesInCreatedKey()
    {
        _apiKeyServiceMock.Setup(s => s.GeneratePlainTextKey(It.IsAny<int>()))
            .Returns("abcdef1234567890");
        _apiKeyServiceMock.Setup(s => s.GenerateSalt(It.IsAny<int>()))
            .Returns("dGVzdA==");
        _apiKeyServiceMock.Setup(s => s.ComputeHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("hash==");

        _repositoryMock.Setup(r =>
                r.UpsertRangeAsync(It.IsAny<UpsertApiKeysRepositoryCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        CreateApiKeyCommand command = new(ApiKeyType.Permanent, ApiKeyRateLimitType.None, Scopes: ["read", "write"]);
        CreateApiKeysCommands commands = new([command]);
        CreateApiKeysCommandHandler handler = CreateHandler();

        Result<PagedResponse<CreateApiKeyData>> result = await handler.Handle(commands, CancellationToken.None);

        Assert.True(result.IsSuccess);
        CreateApiKeyData item = result.Value!.Data.Items.First();
        Assert.Contains("read", item.Scopes);
        Assert.Contains("write", item.Scopes);
    }
}