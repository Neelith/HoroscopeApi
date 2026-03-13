using HoroscopeApi.Application.Infrastructure.Caching;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Domain.Horoscopes.Repositories;
using HoroscopeApi.Infrastructure.HoroscopeService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HoroscopeApi.Tests.Infrastructure.HoroscopeService;

public sealed class HoroscopeQueryServiceTests
{
    private readonly ILogger<HoroscopeQueryService> _logger = NullLogger<HoroscopeQueryService>.Instance;
    private readonly Mock<IHoroscopeRepository> _horoscopeRepoMock = new();
    private readonly Mock<IHoroscopeGeneratorService> _generatorMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IRedisCache> _cacheMock = new();

    private HoroscopeQueryService CreateService() =>
        new(_logger, _horoscopeRepoMock.Object, _generatorMock.Object,
            _unitOfWorkMock.Object, _cacheMock.Object);

    private static Horoscope BuildHoroscope(DateOnly date)
    {
        var signInfo = ZodiacSignInfo.Create(
            ZodiacSign.Aries, "Aries", "♈",
            3, 21, 4, 19,
            Element.Fire, Quality.Cardinal, Polarity.Positive,
            "Mars", "The pioneer.").Value!;

        var horoscope = Horoscope.Create(
            1, HoroscopePeriod.Daily, date,
            "A bold and energetic day lies ahead for Aries.",
            "Love is in the air.", "Career opportunities abound.", "Stay active.",
            [1, 5, 9], ["red", "orange"], 8, ["energy", "passion"]).Value!;

        typeof(Horoscope)
            .GetProperty("ZodiacSignInfo",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)!
            .SetValue(horoscope, signInfo);

        return horoscope;
    }

    private static HoroscopeData BuildHoroscopeData(DateOnly date) => new()
    {
        Sign = "aries",
        SignInfo = new ZodiacSignInfoData
        {
            Name = "Aries", Symbol = "♈", Element = "Fire", Quality = "Cardinal",
            Polarity = "Positive", RulingPlanet = "Mars",
            DateRange = "March 21 - April 19", Description = "The pioneer."
        },
        Period = "daily",
        Date = date,
        Predictions = new HoroscopePredictions
        {
            General = "A bold and energetic day.",
            Love = "Love is in the air.",
            Career = "Career opportunities.",
            Health = "Stay active."
        },
        LuckyNumbers = [1, 5, 9],
        LuckyColors = ["red"],
        MoodScore = 8,
        Keywords = ["energy"]
    };

    [Fact]
    public async Task GetOrGenerateHoroscopeAsync_WhenCacheHit_ReturnsCachedData()
    {
        var date = new DateOnly(2024, 6, 15);
        var cachedData = BuildHoroscopeData(date);

        _cacheMock.Setup(c => c.GetAsync<HoroscopeData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedData);

        var service = CreateService();
        var result = await service.GetOrGenerateHoroscopeAsync(
            ZodiacSign.Aries, HoroscopePeriod.Daily, date, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("true", result.Value!.Attributes?["cached"]);
        Assert.Equal("aries", result.Value.Data.Sign);
    }

    [Fact]
    public async Task GetOrGenerateHoroscopeAsync_WhenCacheHit_DoesNotCallRepository()
    {
        var date = new DateOnly(2024, 6, 15);
        _cacheMock.Setup(c => c.GetAsync<HoroscopeData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildHoroscopeData(date));

        var service = CreateService();
        await service.GetOrGenerateHoroscopeAsync(
            ZodiacSign.Aries, HoroscopePeriod.Daily, date, CancellationToken.None);

        _horoscopeRepoMock.Verify(r => r.GetBySignAndPeriodAsync(
            It.IsAny<GetHoroscopeBySignAndPeriodRepositoryQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetOrGenerateHoroscopeAsync_WhenCacheMissAndRepoHit_ReturnsRepositoryData()
    {
        var date = new DateOnly(2024, 6, 15);
        var horoscope = BuildHoroscope(date);

        _cacheMock.Setup(c => c.GetAsync<HoroscopeData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HoroscopeData?)null);
        _horoscopeRepoMock.Setup(r => r.GetBySignAndPeriodAsync(
                It.IsAny<GetHoroscopeBySignAndPeriodRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(horoscope);
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<HoroscopeData>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();
        var result = await service.GetOrGenerateHoroscopeAsync(
            ZodiacSign.Aries, HoroscopePeriod.Daily, date, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("false", result.Value!.Attributes?["cached"]);
        Assert.Equal("aries", result.Value.Data.Sign);
    }

    [Fact]
    public async Task GetOrGenerateHoroscopeAsync_WhenCacheMissAndRepoHit_StoresInCache()
    {
        var date = new DateOnly(2024, 6, 15);
        var horoscope = BuildHoroscope(date);

        _cacheMock.Setup(c => c.GetAsync<HoroscopeData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HoroscopeData?)null);
        _horoscopeRepoMock.Setup(r => r.GetBySignAndPeriodAsync(
                It.IsAny<GetHoroscopeBySignAndPeriodRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(horoscope);
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<HoroscopeData>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();
        await service.GetOrGenerateHoroscopeAsync(
            ZodiacSign.Aries, HoroscopePeriod.Daily, date, CancellationToken.None);

        _cacheMock.Verify(c => c.SetAsync(
            It.Is<string>(k => k.Contains("Aries") && k.Contains("Daily") && k.Contains("2024-06-15")),
            It.IsAny<HoroscopeData>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrGenerateHoroscopeAsync_WhenRepoMissAndGeneratorSucceeds_ReturnsGeneratedData()
    {
        var date = new DateOnly(2024, 6, 15);
        var horoscope = BuildHoroscope(date);

        _cacheMock.Setup(c => c.GetAsync<HoroscopeData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HoroscopeData?)null);

        _horoscopeRepoMock.SetupSequence(r => r.GetBySignAndPeriodAsync(
                It.IsAny<GetHoroscopeBySignAndPeriodRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Horoscope?)null)
            .ReturnsAsync(horoscope);

        _generatorMock.Setup(g => g.GenerateDailyHoroscopeAsync(
                ZodiacSign.Aries, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(horoscope));

        _horoscopeRepoMock.Setup(r => r.AddAsync(It.IsAny<Horoscope>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<HoroscopeData>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();
        var result = await service.GetOrGenerateHoroscopeAsync(
            ZodiacSign.Aries, HoroscopePeriod.Daily, date, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.Data);
    }

    [Fact]
    public async Task GetOrGenerateHoroscopeAsync_WhenRepoMissAndGeneratorFails_ReturnsNotFoundError()
    {
        var date = new DateOnly(2024, 6, 15);

        _cacheMock.Setup(c => c.GetAsync<HoroscopeData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HoroscopeData?)null);
        _horoscopeRepoMock.Setup(r => r.GetBySignAndPeriodAsync(
                It.IsAny<GetHoroscopeBySignAndPeriodRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Horoscope?)null);
        _generatorMock.Setup(g => g.GenerateDailyHoroscopeAsync(
                It.IsAny<ZodiacSign>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ko<Horoscope>(HoroscopeErrors.NotFound));

        var service = CreateService();
        var result = await service.GetOrGenerateHoroscopeAsync(
            ZodiacSign.Aries, HoroscopePeriod.Daily, date, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Horoscope.NotFound", result.Errors[0].Code);
    }

    [Fact]
    public async Task GetOrGenerateHoroscopeAsync_UsesCacheKeyWithSignPeriodAndDate()
    {
        var date = new DateOnly(2024, 6, 15);
        string? capturedKey = null;

        _cacheMock.Setup(c => c.GetAsync<HoroscopeData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((k, _) => capturedKey = k)
            .ReturnsAsync((HoroscopeData?)null);
        _horoscopeRepoMock.Setup(r => r.GetBySignAndPeriodAsync(
                It.IsAny<GetHoroscopeBySignAndPeriodRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildHoroscope(date));
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<HoroscopeData>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();
        await service.GetOrGenerateHoroscopeAsync(
            ZodiacSign.Aries, HoroscopePeriod.Daily, date, CancellationToken.None);

        Assert.NotNull(capturedKey);
        Assert.Equal("horoscope:Aries:Daily:2024-06-15", capturedKey);
    }
}
