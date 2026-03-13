using HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.HoroscopeService;
using HoroscopeApi.Application.Services.Time;

namespace HoroscopeApi.Tests.Application.Features.Horoscopes;

public sealed class GetDailyHoroscopeHandlerTests
{
    private readonly Mock<IHoroscopeQueryService> _queryServiceMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();

    private GetDailyHoroscopeHandler CreateHandler() =>
        new(_queryServiceMock.Object, _dateTimeProviderMock.Object);

    private static Response<HoroscopeData> BuildHoroscopeResponse()
    {
        var data = new HoroscopeData
        {
            Sign = "aries",
            SignInfo = new ZodiacSignInfoData
            {
                Name = "Aries", Symbol = "♈", Element = "Fire", Quality = "Cardinal",
                Polarity = "Positive", RulingPlanet = "Mars", DateRange = "March 21 - April 19",
                Description = "The first sign."
            },
            Period = "daily",
            Date = new DateOnly(2024, 6, 15),
            Predictions = new HoroscopePredictions { General = "Good day." },
            LuckyNumbers = [1, 7, 13],
            LuckyColors = ["red"],
            MoodScore = 8,
            Keywords = ["energy"]
        };
        return Response<HoroscopeData>.Create(data);
    }

    [Fact]
    public async Task Handle_WithValidSignName_ReturnsHoroscopeData()
    {
        var today = new DateTime(2024, 6, 15);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(today);
        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                ZodiacSign.Aries, HoroscopePeriod.Daily,
                DateOnly.FromDateTime(today), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(BuildHoroscopeResponse()));

        var result = await CreateHandler().Handle(new GetDailyHoroscopeQuery("Aries"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.Data);
        Assert.Equal("aries", result.Value.Data.Sign);
    }

    [Fact]
    public async Task Handle_WithLowercaseSignName_ParsesCorrectly()
    {
        var today = new DateTime(2024, 6, 15);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(today);
        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                ZodiacSign.Taurus, HoroscopePeriod.Daily,
                It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(BuildHoroscopeResponse()));

        var result = await CreateHandler().Handle(new GetDailyHoroscopeQuery("taurus"), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WithInvalidSignName_ReturnsInvalidNameError()
    {
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(DateTime.UtcNow);

        var result = await CreateHandler().Handle(new GetDailyHoroscopeQuery("NotASign"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_UsesTodaysDateFromProvider()
    {
        var specificDate = new DateTime(2024, 3, 21);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(specificDate);

        DateOnly capturedDate = default;
        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                It.IsAny<ZodiacSign>(), It.IsAny<HoroscopePeriod>(),
                It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .Callback<ZodiacSign, HoroscopePeriod, DateOnly, CancellationToken>((_, _, d, _) => capturedDate = d)
            .ReturnsAsync(Result.Ok(BuildHoroscopeResponse()));

        await CreateHandler().Handle(new GetDailyHoroscopeQuery("Aries"), CancellationToken.None);

        Assert.Equal(DateOnly.FromDateTime(specificDate), capturedDate);
    }
}
