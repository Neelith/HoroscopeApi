using HoroscopeApi.Application.Infrastructure.Caching;
using HoroscopeApi.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text;
using System.Text.Json;

namespace HoroscopeApi.Tests.Infrastructure.Caching;

public sealed class RedisCacheTests
{
    private readonly Mock<IDistributedCache> _distributedCacheMock = new();
    private readonly ILogger<RedisCache> _logger = NullLogger<RedisCache>.Instance;

    private RedisCache CreateCache() => new(_distributedCacheMock.Object, _logger);

    [Fact]
    public async Task GetAsync_WithNullKey_ThrowsArgumentNullException()
    {
        var cache = CreateCache();
        await Assert.ThrowsAsync<ArgumentNullException>(() => cache.GetAsync<string>(null!, CancellationToken.None));
    }

    [Fact]
    public async Task GetAsync_WithEmptyKey_ThrowsArgumentNullException()
    {
        var cache = CreateCache();
        await Assert.ThrowsAsync<ArgumentNullException>(() => cache.GetAsync<string>("", CancellationToken.None));
    }

    [Fact]
    public async Task GetAsync_WhenCacheMiss_ReturnsDefault()
    {
        _distributedCacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var cache = CreateCache();
        var result = await cache.GetAsync<string>("test-key", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_WhenCacheHit_ReturnsDeserializedValue()
    {
        var testValue = "hello-world";
        var json = JsonSerializer.Serialize(testValue);
        var bytes = Encoding.UTF8.GetBytes(json);

        _distributedCacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        var cache = CreateCache();
        var result = await cache.GetAsync<string>("test-key", CancellationToken.None);

        Assert.Equal(testValue, result);
    }

    [Fact]
    public async Task GetAsync_WithComplexObject_DeserializesCorrectly()
    {
        var obj = new TestCacheItem { Id = 42, Name = "test" };
        var json = JsonSerializer.Serialize(obj);
        var bytes = Encoding.UTF8.GetBytes(json);

        _distributedCacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        var cache = CreateCache();
        var result = await cache.GetAsync<TestCacheItem>("key", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(42, result!.Id);
        Assert.Equal("test", result.Name);
    }

    [Fact]
    public async Task SetAsync_WithNullKey_ThrowsArgumentNullException()
    {
        var cache = CreateCache();
        await Assert.ThrowsAsync<ArgumentNullException>(() => cache.SetAsync<string>(null!, "value", null, CancellationToken.None));
    }

    [Fact]
    public async Task SetAsync_WithEmptyKey_ThrowsArgumentNullException()
    {
        var cache = CreateCache();
        await Assert.ThrowsAsync<ArgumentNullException>(() => cache.SetAsync<string>("", "value", null, CancellationToken.None));
    }

    [Fact]
    public async Task SetAsync_StoresSerializedValue()
    {
        string? capturedJson = null;
        _distributedCacheMock.Setup(c => c.SetAsync(
                It.IsAny<string>(), It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (_, bytes, _, _) => capturedJson = Encoding.UTF8.GetString(bytes))
            .Returns(Task.CompletedTask);

        var cache = CreateCache();
        await cache.SetAsync("key", "my-value", null, CancellationToken.None);

        Assert.NotNull(capturedJson);
        var deserialized = JsonSerializer.Deserialize<string>(capturedJson!);
        Assert.Equal("my-value", deserialized);
    }

    [Fact]
    public async Task SetAsync_WithCustomExpiration_UsesProvidedExpiration()
    {
        DistributedCacheEntryOptions? capturedOptions = null;
        _distributedCacheMock.Setup(c => c.SetAsync(
                It.IsAny<string>(), It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (_, _, opts, _) => capturedOptions = opts)
            .Returns(Task.CompletedTask);

        var expiration = TimeSpan.FromHours(1);
        var cache = CreateCache();
        await cache.SetAsync("key", "value", expiration, CancellationToken.None);

        Assert.Equal(expiration, capturedOptions!.AbsoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task SetAsync_WithNoExpiration_UsesDefaultFiveMinutes()
    {
        DistributedCacheEntryOptions? capturedOptions = null;
        _distributedCacheMock.Setup(c => c.SetAsync(
                It.IsAny<string>(), It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (_, _, opts, _) => capturedOptions = opts)
            .Returns(Task.CompletedTask);

        var cache = CreateCache();
        await cache.SetAsync("key", "value", null, CancellationToken.None);

        Assert.Equal(TimeSpan.FromMinutes(5), capturedOptions!.AbsoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task RemoveAsync_WithNullKey_ThrowsArgumentNullException()
    {
        var cache = CreateCache();
        await Assert.ThrowsAsync<ArgumentNullException>(() => cache.RemoveAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task RemoveAsync_WithEmptyKey_ThrowsArgumentNullException()
    {
        var cache = CreateCache();
        await Assert.ThrowsAsync<ArgumentNullException>(() => cache.RemoveAsync("", CancellationToken.None));
    }

    [Fact]
    public async Task RemoveAsync_CallsDistributedCacheRemove()
    {
        string? capturedKey = null;
        _distributedCacheMock.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((k, _) => capturedKey = k)
            .Returns(Task.CompletedTask);

        var cache = CreateCache();
        await cache.RemoveAsync("my-key", CancellationToken.None);

        Assert.Equal("my-key", capturedKey);
    }

    private sealed record TestCacheItem
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
