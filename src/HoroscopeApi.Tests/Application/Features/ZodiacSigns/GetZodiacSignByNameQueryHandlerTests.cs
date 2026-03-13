using HoroscopeApi.Application.Features.ZodiacSigns.GetZodiacSignByName;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;

namespace HoroscopeApi.Tests.Application.Features.ZodiacSigns;

public sealed class GetZodiacSignByNameQueryHandlerTests
{
    private readonly Mock<IZodiacSignRepository> _repositoryMock = new();

    private GetZodiacSignByNameQueryHandler CreateHandler() => new(_repositoryMock.Object);

    private static ZodiacSignInfo BuildSignInfo(ZodiacSign sign = ZodiacSign.Aries) =>
        ZodiacSignInfo.Create(sign, "Aries", "♈", 3, 21, 4, 19,
            Element.Fire, Quality.Cardinal, Polarity.Positive, "Mars", "The pioneer.").Value!;

    [Fact]
    public async Task Handle_WithValidSignName_ReturnsSignData()
    {
        var signInfo = BuildSignInfo();
        _repositoryMock.Setup(r => r.GetBySignAsync(It.IsAny<GetZodiacSignBySignRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(signInfo);

        var result = await CreateHandler().Handle(new GetZodiacSignByNameQuery("Aries"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.Data);
        Assert.Equal("Aries", result.Value.Data.Name);
        Assert.Equal("♈", result.Value.Data.Symbol);
    }

    [Fact]
    public async Task Handle_WithInvalidSignName_ReturnsInvalidNameError()
    {
        var result = await CreateHandler().Handle(new GetZodiacSignByNameQuery("InvalidSign"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_WhenSignNotFoundInRepository_ReturnsNotFoundError()
    {
        _repositoryMock.Setup(r => r.GetBySignAsync(It.IsAny<GetZodiacSignBySignRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ZodiacSignInfo?)null);

        var result = await CreateHandler().Handle(new GetZodiacSignByNameQuery("Aries"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.NotFound", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_PassesCorrectSignToRepository()
    {
        ZodiacSign capturedSign = default;
        _repositoryMock.Setup(r => r.GetBySignAsync(It.IsAny<GetZodiacSignBySignRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .Callback<GetZodiacSignBySignRepositoryQuery, CancellationToken>((q, _) => capturedSign = q.Sign)
            .ReturnsAsync((ZodiacSignInfo?)null);

        await CreateHandler().Handle(new GetZodiacSignByNameQuery("Taurus"), CancellationToken.None);

        Assert.Equal(ZodiacSign.Taurus, capturedSign);
    }

    [Fact]
    public async Task Handle_MapsDataCorrectly()
    {
        var signInfo = ZodiacSignInfo.Create(
            ZodiacSign.Gemini, "Gemini", "♊",
            5, 21, 6, 20,
            Element.Air, Quality.Mutable, Polarity.Positive,
            "Mercury", "The twins.").Value!;

        _repositoryMock.Setup(r => r.GetBySignAsync(It.IsAny<GetZodiacSignBySignRepositoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(signInfo);

        var result = await CreateHandler().Handle(new GetZodiacSignByNameQuery("Gemini"), CancellationToken.None);

        var data = result.Value!.Data;
        Assert.Equal("Gemini", data.Name);
        Assert.Equal("♊", data.Symbol);
        Assert.Equal("Air", data.Element);
        Assert.Equal("Mutable", data.Quality);
        Assert.Equal("Positive", data.Polarity);
        Assert.Equal("Mercury", data.RulingPlanet);
        Assert.Equal("The twins.", data.Description);
    }
}
