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

    private static Response<CompatibilityData> BuildCompatibilityResponse()
    {
        CompatibilityData data = new()
        {
            FirstSign = "aries",
            FirstSignInfo =
                new ZodiacSignInfoData
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
            SecondSign = "taurus",
            SecondSignInfo = new ZodiacSignInfoData
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
        return Response<CompatibilityData>.Create(data);
    }

    [Fact]
    public async Task Handle_WithValidSignNames_ReturnsCompatibilityData()
    {
        _queryServiceMock.Setup(s => s.GetOrGenerateCompatibilityAsync(
                ZodiacSign.Aries, ZodiacSign.Taurus, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(BuildCompatibilityResponse()));

        Result<Response<CompatibilityData>> result = await CreateHandler().Handle(
            new GetCompatibilityQuery("Aries", "Taurus"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.Data);
        Assert.Equal("aries", result.Value.Data.FirstSign);
        Assert.Equal("taurus", result.Value.Data.SecondSign);
        Assert.Equal(72, result.Value.Data.Score);
    }

    [Fact]
    public async Task Handle_WithLowercaseSignNames_ParsesCorrectly()
    {
        _queryServiceMock.Setup(s => s.GetOrGenerateCompatibilityAsync(
                ZodiacSign.Leo, ZodiacSign.Virgo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(BuildCompatibilityResponse()));

        Result<Response<CompatibilityData>> result = await CreateHandler().Handle(
            new GetCompatibilityQuery("leo", "virgo"), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WithInvalidFirstSignName_ReturnsInvalidNameError()
    {
        Result<Response<CompatibilityData>> result = await CreateHandler().Handle(
            new GetCompatibilityQuery("NotASign", "Aries"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_WithInvalidSecondSignName_ReturnsInvalidNameError()
    {
        Result<Response<CompatibilityData>> result = await CreateHandler().Handle(
            new GetCompatibilityQuery("Aries", "NotASign"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_WithBothInvalidSignNames_ReturnsInvalidNameError()
    {
        Result<Response<CompatibilityData>> result = await CreateHandler().Handle(
            new GetCompatibilityQuery("Foo", "Bar"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_DelegatesCorrectlyToQueryService()
    {
        _queryServiceMock.Setup(s => s.GetOrGenerateCompatibilityAsync(
                ZodiacSign.Pisces, ZodiacSign.Scorpio, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(BuildCompatibilityResponse()));

        await CreateHandler().Handle(
            new GetCompatibilityQuery("Pisces", "Scorpio"), CancellationToken.None);

        _queryServiceMock.Verify(s => s.GetOrGenerateCompatibilityAsync(
            ZodiacSign.Pisces, ZodiacSign.Scorpio, It.IsAny<CancellationToken>()), Times.Once);
    }
}