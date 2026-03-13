using HoroscopeApi.Application.Features.ZodiacSigns.GetAllZodiacSigns;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.ZodiacSigns.Repositories;

namespace HoroscopeApi.Tests.Application.Features.ZodiacSigns;

public sealed class GetAllZodiacSignsQueryHandlerTests
{
    private readonly Mock<IZodiacSignRepository> _repositoryMock = new();

    private GetAllZodiacSignsQueryHandler CreateHandler() => new(_repositoryMock.Object);

    private static ZodiacSignInfo BuildZodiacSignInfo(ZodiacSign sign, string name) =>
        ZodiacSignInfo.Create(sign, name, "♈", 3, 21, 4, 19,
            Element.Fire, Quality.Cardinal, Polarity.Positive, "Mars", $"{name} description.").Value!;

    [Fact]
    public async Task Handle_WhenRepositoryReturnsData_ReturnsMappedPagedResponse()
    {
        var signs = new List<ZodiacSignInfo>
        {
            BuildZodiacSignInfo(ZodiacSign.Aries, "Aries"),
            BuildZodiacSignInfo(ZodiacSign.Taurus, "Taurus"),
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(signs);

        var result = await CreateHandler().Handle(new GetAllZodiacSignsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var items = result.Value!.Data.Items.ToList();
        Assert.Equal(2, items.Count);
        Assert.Equal("Aries", items[0].Name);
        Assert.Equal("Taurus", items[1].Name);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmpty_ReturnsEmptyPagedResponse()
    {
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await CreateHandler().Handle(new GetAllZodiacSignsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Data.Items);
    }

    [Fact]
    public async Task Handle_MapsZodiacSignInfoDataCorrectly()
    {
        var signInfo = ZodiacSignInfo.Create(
            ZodiacSign.Leo, "Leo", "♌",
            7, 23, 8, 22,
            Element.Fire, Quality.Fixed, Polarity.Positive,
            "Sun", "The lion.").Value!;

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([signInfo]);

        var result = await CreateHandler().Handle(new GetAllZodiacSignsQuery(), CancellationToken.None);

        var item = result.Value!.Data.Items.First();
        Assert.Equal("Leo", item.Name);
        Assert.Equal("♌", item.Symbol);
        Assert.Equal("Fire", item.Element);
        Assert.Equal("Fixed", item.Quality);
        Assert.Equal("Positive", item.Polarity);
        Assert.Equal("Sun", item.RulingPlanet);
        Assert.Equal("The lion.", item.Description);
        Assert.Contains("July", item.DateRange);
        Assert.Contains("August", item.DateRange);
    }
}
