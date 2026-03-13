using HoroscopeApi.Application.Features.ApiKeys.CreateApiKeys;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.Domain.ApiKeys.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HoroscopeApi.Tests.Application.Features.ApiKeys;

public sealed class CreateApiKeysCommandHandlerTests
{
    private readonly ILogger<CreateApiKeysCommandHandler> _logger = NullLogger<CreateApiKeysCommandHandler>.Instance;
    private readonly Mock<IApiKeyRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IApiKeyService> _apiKeyServiceMock = new();

    private CreateApiKeysCommandHandler CreateHandler() =>
        new(_logger, _repositoryMock.Object, _unitOfWorkMock.Object, _apiKeyServiceMock.Object);

    [Fact]
    public async Task Handle_WithValidCommands_ReturnsPagedResponseWithCreatedKeys()
    {
        _apiKeyServiceMock.Setup(s => s.GeneratePlainTextKey(It.IsAny<int>()))
            .Returns("abcdef1234567890");
        _apiKeyServiceMock.Setup(s => s.GenerateSalt(It.IsAny<int>()))
            .Returns("dGVzdA==");
        _apiKeyServiceMock.Setup(s => s.ComputeHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("hashvalue==");

        _repositoryMock.Setup(r => r.UpsertRangeAsync(It.IsAny<UpsertApiKeysRepositoryCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.None);
        var commands = new CreateApiKeysCommands([command]);
        var handler = CreateHandler();

        var result = await handler.Handle(commands, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var items = result.Value!.Data.Items.ToList();
        Assert.Single(items);
        Assert.Equal("abcdef12", items[0].Prefix);
        Assert.Equal("abcdef1234567890", items[0].PlainTextKey);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_ReturnsAllCreatedKeys()
    {
        var callCount = 0;
        _apiKeyServiceMock.Setup(s => s.GeneratePlainTextKey(It.IsAny<int>()))
            .Returns(() => callCount++ == 0 ? "abcdef1234567890" : "1234567890abcdef");
        _apiKeyServiceMock.Setup(s => s.GenerateSalt(It.IsAny<int>()))
            .Returns("dGVzdA==");
        _apiKeyServiceMock.Setup(s => s.ComputeHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("hash==");

        _repositoryMock.Setup(r => r.UpsertRangeAsync(It.IsAny<UpsertApiKeysRepositoryCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var commands = new CreateApiKeysCommands([
            new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.None),
            new CreateApiKeyCommand(ApiKeyType.Temporary, ApiKeyRateLimitType.PerMinute, 100, 60)
        ]);
        var handler = CreateHandler();

        var result = await handler.Handle(commands, CancellationToken.None);

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

        _repositoryMock.Setup(r => r.UpsertRangeAsync(It.IsAny<UpsertApiKeysRepositoryCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var commands = new CreateApiKeysCommands([
            new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.None)
        ]);
        var handler = CreateHandler();

        var result = await handler.Handle(commands, CancellationToken.None);

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

        _repositoryMock.Setup(r => r.UpsertRangeAsync(It.IsAny<UpsertApiKeysRepositoryCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateApiKeyCommand(ApiKeyType.Permanent, ApiKeyRateLimitType.None, Scopes: ["read", "write"]);
        var commands = new CreateApiKeysCommands([command]);
        var handler = CreateHandler();

        var result = await handler.Handle(commands, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var item = result.Value!.Data.Items.First();
        Assert.Contains("read", item.Scopes);
        Assert.Contains("write", item.Scopes);
    }
}
