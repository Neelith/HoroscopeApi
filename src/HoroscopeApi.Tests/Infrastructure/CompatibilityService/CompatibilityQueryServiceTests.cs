using System.Reflection;
using HoroscopeApi.Application.Infrastructure.Caching;
using HoroscopeApi.Application.Infrastructure.Persistance;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Application.Services.CompatibilityService;
using HoroscopeApi.Domain.Compatibilities;
using HoroscopeApi.Domain.Compatibilities.Repositories;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;
using HoroscopeApi.Infrastructure.CompatibilityService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HoroscopeApi.Tests.Infrastructure.CompatibilityService;

public sealed class CompatibilityQueryServiceTests
{
    private readonly Mock<IRedisCache> _cacheMock = new();

    private readonly Mock<ICompatibilityRepository> _compatibilityRepoMock = new();
    private readonly Mock<ICompatibilityGeneratorService> _generatorMock = new();

    private readonly ILogger<CompatibilityQueryService> _logger =
        NullLogger<CompatibilityQueryService>.Instance;

    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IZodiacSignRepository> _zodiacSignRepoMock = new();

    private CompatibilityQueryService CreateService()
    {
        return new CompatibilityQueryService(_logger, _compatibilityRepoMock.Object, _generatorMock.Object,
            _zodiacSignRepoMock.Object, _unitOfWorkMock.Object, _cacheMock.Object);
    }

    private static ZodiacSignInfo BuildSignInfo(ZodiacSign sign, string name, string symbol,
        int startMonth, int startDay, int endMonth, int endDay,
        Element element, Quality quality, Polarity polarity,
        string rulingPlanet, string description)
    {
        ZodiacSignInfo signInfo = ZodiacSignInfo.Create(
            sign, name, symbol, startMonth, startDay, endMonth, endDay,
            element, quality, polarity, rulingPlanet, description).Value!;

        typeof(ZodiacSignInfo)
            .GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(signInfo, (int)sign);

        return signInfo;
    }

    private static ZodiacSignInfo BuildAriesSignInfo()
    {
        return BuildSignInfo(ZodiacSign.Aries, "Aries", "♈", 3, 21, 4, 19,
            Element.Fire, Quality.Cardinal, Polarity.Positive, "Mars", "The pioneer.");
    }

    private static ZodiacSignInfo BuildTaurusSignInfo()
    {
        return BuildSignInfo(ZodiacSign.Taurus, "Taurus", "♉", 4, 20, 5, 20,
            Element.Earth, Quality.Fixed, Polarity.Negative, "Venus", "The builder.");
    }

    private static Compatibility BuildCompatibility(ZodiacSignInfo first, ZodiacSignInfo second)
    {
        Compatibility compatibility = Compatibility.Create(
            first.Id, second.Id, 75, "A strong and balanced pairing.").Value!;

        typeof(Compatibility)
            .GetProperty("FirstZodiacSignInfo",
                BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(compatibility, first);

        typeof(Compatibility)
            .GetProperty("SecondZodiacSignInfo",
                BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(compatibility, second);

        return compatibility;
    }

    private static CompatibilityData BuildCompatibilityData()
    {
        return new CompatibilityData
        {
            FirstZodiacSign =
                new ZodiacSignData
                {
                    Name = "Aries",
                    Symbol = "♈",
                    Element = "Fire",
                    Quality = "Cardinal",
                    Polarity = "Positive",
                    RulingPlanet = "Mars",
                    DateRange = "March 21 - April 19",
                    Description = "The pioneer."
                },
            SecondZodiacSign = new ZodiacSignData
            {
                Name = "Taurus",
                Symbol = "♉",
                Element = "Earth",
                Quality = "Fixed",
                Polarity = "Negative",
                RulingPlanet = "Venus",
                DateRange = "April 20 - May 20",
                Description = "The builder."
            },
            Score = 75,
            Description = "A strong and balanced pairing."
        };
    }

    private void SetupZodiacSignRepos(ZodiacSignInfo aries, ZodiacSignInfo taurus)
    {
        _zodiacSignRepoMock.Setup(r => r.GetBySignAsync(
                It.Is<GetZodiacSignBySignRepositoryQuery>(q => q.Sign == ZodiacSign.Aries),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(aries);
        _zodiacSignRepoMock.Setup(r => r.GetBySignAsync(
                It.Is<GetZodiacSignBySignRepositoryQuery>(q => q.Sign == ZodiacSign.Taurus),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(taurus);
    }

    [Fact]
    public async Task GetOrGenerateCompatibilityAsync_WhenCacheHit_ReturnsCachedData()
    {
        CompatibilityData cachedData = BuildCompatibilityData();

        _cacheMock.Setup(c => c.GetAsync<CompatibilityData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedData);

        CompatibilityQueryService service = CreateService();
        var result = await service.GetOrGenerateCompatibilityAsync(
            ZodiacSign.Aries, ZodiacSign.Taurus, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.IsCached);
        Assert.Equal("Aries", result.Value.Data.FirstZodiacSign.Name);
    }

    [Fact]
    public async Task GetOrGenerateCompatibilityAsync_WhenCacheHit_DoesNotCallRepository()
    {
        _cacheMock.Setup(c => c.GetAsync<CompatibilityData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildCompatibilityData());

        CompatibilityQueryService service = CreateService();
        await service.GetOrGenerateCompatibilityAsync(
            ZodiacSign.Aries, ZodiacSign.Taurus, CancellationToken.None);

        _compatibilityRepoMock.Verify(r => r.GetBySignPairAsync(
            It.IsAny<GetCompatibilityBySignPairRepositoryQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetOrGenerateCompatibilityAsync_WhenCacheMissAndRepoHit_ReturnsRepositoryData()
    {
        ZodiacSignInfo aries = BuildAriesSignInfo();
        ZodiacSignInfo taurus = BuildTaurusSignInfo();
        Compatibility compatibility = BuildCompatibility(aries, taurus);

        _cacheMock.Setup(c => c.GetAsync<CompatibilityData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompatibilityData?)null);
        _cacheMock.Setup(c => c.TryAcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("lock-value");
        SetupZodiacSignRepos(aries, taurus);
        _compatibilityRepoMock.Setup(r => r.GetBySignPairAsync(
                It.IsAny<GetCompatibilityBySignPairRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(compatibility);
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<CompatibilityData>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _cacheMock.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        CompatibilityQueryService service = CreateService();
        var result = await service.GetOrGenerateCompatibilityAsync(
            ZodiacSign.Aries, ZodiacSign.Taurus, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.IsCached);
        Assert.Equal("Aries", result.Value.Data.FirstZodiacSign.Name);
    }

    [Fact]
    public async Task GetOrGenerateCompatibilityAsync_WhenCacheMissAndRepoHit_StoresInCache()
    {
        ZodiacSignInfo aries = BuildAriesSignInfo();
        ZodiacSignInfo taurus = BuildTaurusSignInfo();
        Compatibility compatibility = BuildCompatibility(aries, taurus);

        _cacheMock.Setup(c => c.GetAsync<CompatibilityData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompatibilityData?)null);
        _cacheMock.Setup(c => c.TryAcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("lock-value");
        SetupZodiacSignRepos(aries, taurus);
        _compatibilityRepoMock.Setup(r => r.GetBySignPairAsync(
                It.IsAny<GetCompatibilityBySignPairRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(compatibility);
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<CompatibilityData>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _cacheMock.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        CompatibilityQueryService service = CreateService();
        await service.GetOrGenerateCompatibilityAsync(
            ZodiacSign.Aries, ZodiacSign.Taurus, CancellationToken.None);

        _cacheMock.Verify(c => c.SetAsync(
            It.Is<string>(k => k.Contains("1") && k.Contains("2")),
            It.IsAny<CompatibilityData>(),
            It.Is<TimeSpan?>(t => t == TimeSpan.FromHours(24)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrGenerateCompatibilityAsync_WhenRepoMissAndGeneratorSucceeds_ReturnsGeneratedData()
    {
        ZodiacSignInfo aries = BuildAriesSignInfo();
        ZodiacSignInfo taurus = BuildTaurusSignInfo();
        Compatibility compatibility = BuildCompatibility(aries, taurus);

        _cacheMock.Setup(c => c.GetAsync<CompatibilityData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompatibilityData?)null);
        _cacheMock.Setup(c => c.TryAcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("lock-value");
        SetupZodiacSignRepos(aries, taurus);

        _compatibilityRepoMock.SetupSequence(r => r.GetBySignPairAsync(
                It.IsAny<GetCompatibilityBySignPairRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Compatibility?)null)
            .ReturnsAsync(compatibility);

        _generatorMock.Setup(g => g.GenerateCompatibilityAsync(
                aries, taurus, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Ok(new HuggingFaceCompatibilityData("Aries", "Taurus", 75, "A strong and balanced pairing.")));

        _compatibilityRepoMock.Setup(r => r.AddAsync(It.IsAny<Compatibility>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<CompatibilityData>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _cacheMock.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        CompatibilityQueryService service = CreateService();
        var result = await service.GetOrGenerateCompatibilityAsync(
            ZodiacSign.Aries, ZodiacSign.Taurus, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.Data);
    }

    [Fact]
    public async Task GetOrGenerateCompatibilityAsync_WhenRepoMissAndGeneratorFails_ReturnsNotFoundError()
    {
        ZodiacSignInfo aries = BuildAriesSignInfo();
        ZodiacSignInfo taurus = BuildTaurusSignInfo();

        _cacheMock.Setup(c => c.GetAsync<CompatibilityData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompatibilityData?)null);
        _cacheMock.Setup(c => c.TryAcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("lock-value");
        SetupZodiacSignRepos(aries, taurus);
        _compatibilityRepoMock.Setup(r => r.GetBySignPairAsync(
                It.IsAny<GetCompatibilityBySignPairRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Compatibility?)null);
        _generatorMock.Setup(g => g.GenerateCompatibilityAsync(
                It.IsAny<ZodiacSignInfo>(), It.IsAny<ZodiacSignInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ko<HuggingFaceCompatibilityData>(CompatibilityErrors.NotFound));
        _cacheMock.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        CompatibilityQueryService service = CreateService();
        var result = await service.GetOrGenerateCompatibilityAsync(
            ZodiacSign.Aries, ZodiacSign.Taurus, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Compatibility.NotFound", result.Errors[0].Code);
    }

    [Fact]
    public async Task GetOrGenerateCompatibilityAsync_NormalizesSignPair_LowerEnumFirst()
    {
        string? capturedKey = null;

        _cacheMock.Setup(c => c.GetAsync<CompatibilityData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((k, _) => capturedKey = k)
            .ReturnsAsync(BuildCompatibilityData());

        CompatibilityQueryService service = CreateService();
        await service.GetOrGenerateCompatibilityAsync(
            ZodiacSign.Taurus, ZodiacSign.Aries, CancellationToken.None);

        Assert.NotNull(capturedKey);
        Assert.Equal("compatibility:1:2", capturedKey);
    }

    [Fact]
    public async Task GetOrGenerateCompatibilityAsync_WhenFirstSignNotFound_ReturnsError()
    {
        _cacheMock.Setup(c => c.GetAsync<CompatibilityData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompatibilityData?)null);
        _cacheMock.Setup(c => c.TryAcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("lock-value");
        _zodiacSignRepoMock.Setup(r => r.GetBySignAsync(
                It.IsAny<GetZodiacSignBySignRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ZodiacSignInfo?)null);
        _cacheMock.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        CompatibilityQueryService service = CreateService();
        var result = await service.GetOrGenerateCompatibilityAsync(
            ZodiacSign.Aries, ZodiacSign.Taurus, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("ZodiacSign", result.Errors[0].Code);
    }

    [Fact]
    public async Task GetOrGenerateCompatibilityAsync_UsesCacheKeyWithNormalizedSigns()
    {
        string? capturedKey = null;
        ZodiacSignInfo aries = BuildAriesSignInfo();
        ZodiacSignInfo taurus = BuildTaurusSignInfo();
        Compatibility compatibility = BuildCompatibility(aries, taurus);

        _cacheMock.Setup(c => c.GetAsync<CompatibilityData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((k, _) => capturedKey = k)
            .ReturnsAsync((CompatibilityData?)null);
        _cacheMock.Setup(c => c.TryAcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("lock-value");
        SetupZodiacSignRepos(aries, taurus);
        _compatibilityRepoMock.Setup(r => r.GetBySignPairAsync(
                It.IsAny<GetCompatibilityBySignPairRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(compatibility);
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<CompatibilityData>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _cacheMock.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        CompatibilityQueryService service = CreateService();
        await service.GetOrGenerateCompatibilityAsync(
            ZodiacSign.Aries, ZodiacSign.Taurus, CancellationToken.None);

        Assert.NotNull(capturedKey);
        Assert.Equal("compatibility:1:2", capturedKey);
    }
}