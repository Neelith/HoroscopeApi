using System.Text;
using HoroscopeApi.Application.Services.ApiKey;
using HoroscopeApi.Application.Services.Time;
using HoroscopeApi.Infrastructure.ApiKey;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HoroscopeApi.Tests.Infrastructure.ApiKey;

public sealed class RedisApiKeyRateLimiterTests
{
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly Mock<IDistributedCache> _distributedCacheMock = new();
    private readonly ILogger<RedisApiKeyRateLimiter> _logger = NullLogger<RedisApiKeyRateLimiter>.Instance;

    private RedisApiKeyRateLimiter CreateRateLimiter()
    {
        return new RedisApiKeyRateLimiter(
            _distributedCacheMock.Object,
            _dateTimeProviderMock.Object,
            _logger);
    }

    [Fact]
    public async Task CheckRateLimitAsync_FirstRequest_IsAllowed()
    {
        DateTime now = new(2026, 3, 28, 14, 35, 20, DateTimeKind.Utc);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);
        _distributedCacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        RedisApiKeyRateLimiter rateLimiter = CreateRateLimiter();

        ApiKeyRateLimitResult result = await rateLimiter.CheckRateLimitAsync(
            42, ApiKeyRateLimitType.PerMinute, 100, CancellationToken.None);

        Assert.True(result.IsAllowed);
        Assert.Equal(100, result.Limit);
        Assert.Equal(99, result.Remaining);
    }

    [Fact]
    public async Task CheckRateLimitAsync_AtLimit_IsNotAllowed()
    {
        DateTime now = new(2026, 3, 28, 14, 35, 20, DateTimeKind.Utc);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);

        // Simulate counter already at limit (100)
        byte[] countBytes = "100"u8.ToArray();
        _distributedCacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countBytes);

        RedisApiKeyRateLimiter rateLimiter = CreateRateLimiter();

        ApiKeyRateLimitResult result = await rateLimiter.CheckRateLimitAsync(
            42, ApiKeyRateLimitType.PerMinute, 100, CancellationToken.None);

        Assert.True(result.IsExceeded);
        Assert.Equal(100, result.Limit);
        Assert.Equal(0, result.Remaining);
    }

    [Fact]
    public async Task CheckRateLimitAsync_BelowLimit_IsAllowed()
    {
        DateTime now = new(2026, 3, 28, 14, 35, 20, DateTimeKind.Utc);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);

        byte[] countBytes = "50"u8.ToArray();
        _distributedCacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countBytes);

        RedisApiKeyRateLimiter rateLimiter = CreateRateLimiter();

        ApiKeyRateLimitResult result = await rateLimiter.CheckRateLimitAsync(
            42, ApiKeyRateLimitType.PerMinute, 100, CancellationToken.None);

        Assert.True(result.IsAllowed);
        Assert.Equal(100, result.Limit);
        Assert.Equal(49, result.Remaining);
    }

    [Fact]
    public void ComputeWindow_PerMinute_FormatsIso8601()
    {
        DateTime now = new(2026, 3, 28, 14, 35, 20, DateTimeKind.Utc);

        (string windowKey, TimeSpan windowDuration, DateTime resetAtUtc) =
            RedisApiKeyRateLimiter.ComputeWindow(42, ApiKeyRateLimitType.PerMinute, now);

        Assert.Equal("ratelimit:42:2026-03-28T14:35", windowKey);
        Assert.Equal(TimeSpan.FromSeconds(60), windowDuration);
        Assert.Equal(new DateTime(2026, 3, 28, 14, 36, 0, DateTimeKind.Utc), resetAtUtc);
    }

    [Fact]
    public void ComputeWindow_PerHour_FormatsIso8601()
    {
        DateTime now = new(2026, 3, 28, 14, 35, 20, DateTimeKind.Utc);

        (string windowKey, TimeSpan windowDuration, DateTime resetAtUtc) =
            RedisApiKeyRateLimiter.ComputeWindow(42, ApiKeyRateLimitType.PerHour, now);

        Assert.Equal("ratelimit:42:2026-03-28T14", windowKey);
        Assert.Equal(TimeSpan.FromSeconds(3600), windowDuration);
        Assert.Equal(new DateTime(2026, 3, 28, 15, 0, 0, DateTimeKind.Utc), resetAtUtc);
    }

    [Fact]
    public void ComputeWindow_PerDay_FormatsIso8601()
    {
        DateTime now = new(2026, 3, 28, 14, 35, 20, DateTimeKind.Utc);

        (string windowKey, TimeSpan windowDuration, DateTime resetAtUtc) =
            RedisApiKeyRateLimiter.ComputeWindow(42, ApiKeyRateLimitType.PerDay, now);

        Assert.Equal("ratelimit:42:2026-03-28", windowKey);
        Assert.Equal(TimeSpan.FromSeconds(86400), windowDuration);
        Assert.Equal(new DateTime(2026, 3, 29, 0, 0, 0, DateTimeKind.Utc), resetAtUtc);
    }

    [Fact]
    public void ComputeWindow_None_ThrowsArgumentOutOfRangeException()
    {
        DateTime now = new(2026, 3, 28, 14, 35, 20, DateTimeKind.Utc);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            RedisApiKeyRateLimiter.ComputeWindow(42, ApiKeyRateLimitType.None, now));
    }

    [Fact]
    public async Task CheckRateLimitAsync_PerHour_ReturnsCorrectResetTime()
    {
        DateTime now = new(2026, 3, 28, 14, 35, 20, DateTimeKind.Utc);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);
        _distributedCacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        RedisApiKeyRateLimiter rateLimiter = CreateRateLimiter();

        ApiKeyRateLimitResult result = await rateLimiter.CheckRateLimitAsync(
            42, ApiKeyRateLimitType.PerHour, 1000, CancellationToken.None);

        Assert.Equal(new DateTime(2026, 3, 28, 15, 0, 0, DateTimeKind.Utc), result.ResetAtUtc);
    }

    [Fact]
    public async Task CheckRateLimitAsync_PerDay_ReturnsCorrectResetTime()
    {
        DateTime now = new(2026, 3, 28, 14, 35, 20, DateTimeKind.Utc);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);
        _distributedCacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        RedisApiKeyRateLimiter rateLimiter = CreateRateLimiter();

        ApiKeyRateLimitResult result = await rateLimiter.CheckRateLimitAsync(
            42, ApiKeyRateLimitType.PerDay, 10000, CancellationToken.None);

        Assert.Equal(new DateTime(2026, 3, 29, 0, 0, 0, DateTimeKind.Utc), result.ResetAtUtc);
    }

    [Fact]
    public async Task CheckRateLimitAsync_StoresIncrementedCounter()
    {
        DateTime now = new(2026, 3, 28, 14, 35, 20, DateTimeKind.Utc);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);

        byte[] countBytes = "5"u8.ToArray();
        _distributedCacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countBytes);

        RedisApiKeyRateLimiter rateLimiter = CreateRateLimiter();

        await rateLimiter.CheckRateLimitAsync(
            42, ApiKeyRateLimitType.PerMinute, 100, CancellationToken.None);

        _distributedCacheMock.Verify(c => c.SetAsync(
                It.Is<string>(k => k == "ratelimit:42:2026-03-28T14:35"),
                It.Is<byte[]>(v => Encoding.UTF8.GetString(v) == "6"),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}