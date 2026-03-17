using HoroscopeApi.Application.Features.Compatibilities.GetCompatibility;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.CompatibilityService;

namespace HoroscopeApi.Tests.Application.Features.Compatibilities;

public sealed class GetCompatibilityQueryHandlerTests
{
    private readonly Mock<ICompatibilityQueryService> _queryServiceMock = new();

    private GetCompatibilityQueryHandler CreateHandler()
    {
        return new GetCompatibilityQueryHandler(_queryServiceMock.Object);
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
            Score = 72,
            Description = "A strong and balanced pairing."
        };
    }

    [Fact]
    public async Task Handle_WithValidSignNames_ReturnsCompatibilityData()
    {
        _queryServiceMock.Setup(s => s.GetOrGenerateCompatibilityAsync(
                ZodiacSign.Aries, ZodiacSign.Taurus, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok((BuildCompatibilityData(), false)));

        var result = await CreateHandler().Handle(
            new GetCompatibilityQuery("Aries", "Taurus"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.Data);
        Assert.Equal("Aries", result.Value.Data.FirstZodiacSign.Name);
        Assert.Equal("Taurus", result.Value.Data.SecondZodiacSign.Name);
        Assert.Equal(72, result.Value.Data.Score);
    }

    [Fact]
    public async Task Handle_WithLowercaseSignNames_ParsesCorrectly()
    {
        _queryServiceMock.Setup(s => s.GetOrGenerateCompatibilityAsync(
                ZodiacSign.Leo, ZodiacSign.Virgo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok((BuildCompatibilityData(), false)));

        var result = await CreateHandler().Handle(
            new GetCompatibilityQuery("leo", "virgo"), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WithInvalidFirstSignName_ReturnsInvalidNameError()
    {
        var result = await CreateHandler().Handle(
            new GetCompatibilityQuery("NotASign", "Aries"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_WithInvalidSecondSignName_ReturnsInvalidNameError()
    {
        var result = await CreateHandler().Handle(
            new GetCompatibilityQuery("Aries", "NotASign"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_WithBothInvalidSignNames_ReturnsInvalidNameError()
    {
        var result = await CreateHandler().Handle(
            new GetCompatibilityQuery("Foo", "Bar"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_DelegatesCorrectlyToQueryService()
    {
        _queryServiceMock.Setup(s => s.GetOrGenerateCompatibilityAsync(
                ZodiacSign.Pisces, ZodiacSign.Scorpio, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok((BuildCompatibilityData(), false)));

        await CreateHandler().Handle(
            new GetCompatibilityQuery("Pisces", "Scorpio"), CancellationToken.None);

        _queryServiceMock.Verify(s => s.GetOrGenerateCompatibilityAsync(
            ZodiacSign.Pisces, ZodiacSign.Scorpio, It.IsAny<CancellationToken>()), Times.Once);
    }
}