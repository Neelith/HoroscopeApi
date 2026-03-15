using HoroscopeApi.Application.Features.Horoscopes.GetMonthlyHoroscope;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.HoroscopeService;
using HoroscopeApi.Application.Services.Time;

namespace HoroscopeApi.Tests.Application.Features.Horoscopes;

public sealed class GetMonthlyHoroscopeQueryHandlerTests
{
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly Mock<IHoroscopeQueryService> _queryServiceMock = new();

    private GetMonthlyHoroscopeQueryHandler CreateHandler()
    {
        return new GetMonthlyHoroscopeQueryHandler(_queryServiceMock.Object, _dateTimeProviderMock.Object);
    }

    private static Response<HoroscopeData> BuildResponse() =>
        Response<HoroscopeData>.Create(new HoroscopeData
        {
            Sign = "scorpio",
            Period = "monthly",
            Date = new DateOnly(2024, 11, 1),
            SignInfo = new ZodiacSignInfoData
            {
                Name = "Scorpio",
                Symbol = "♏",
                Element = "Water",
                Quality = "Fixed",
                Polarity = "Negative",
                RulingPlanet = "Pluto",
                DateRange = "October 23 - November 21",
                Description = "Deep."
            },
            Predictions = new HoroscopePredictions { General = "Transform." },
            LuckyNumbers = [8, 11, 18],
            LuckyColors = ["black"],
            MoodScore = 6,
            Keywords = ["transformation"]
        });

    [Fact]
    public async Task Handle_WithValidSignName_ReturnsHoroscopeData()
    {
        var now = new DateTime(2024, 11, 1);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);
        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                ZodiacSign.Scorpio, HoroscopePeriod.Monthly,
                DateOnly.FromDateTime(now), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(BuildResponse()));

        var result = await CreateHandler().Handle(new GetMonthlyHoroscopeQuery("Scorpio"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("monthly", result.Value!.Data.Period);
    }

    [Fact]
    public async Task Handle_WithInvalidSignName_ReturnsInvalidNameError()
    {
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(DateTime.UtcNow);

        var result = await CreateHandler().Handle(new GetMonthlyHoroscopeQuery("Bogus"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_UsesPeriodMonthly()
    {
        var now = DateTime.UtcNow;
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);
        HoroscopePeriod capturedPeriod = default;

        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                It.IsAny<ZodiacSign>(), It.IsAny<HoroscopePeriod>(),
                It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .Callback<ZodiacSign, HoroscopePeriod, DateOnly, CancellationToken>((_, p, _, _) => capturedPeriod = p)
            .ReturnsAsync(Result.Ok(BuildResponse()));

        await CreateHandler().Handle(new GetMonthlyHoroscopeQuery("Aries"), CancellationToken.None);

        Assert.Equal(HoroscopePeriod.Monthly, capturedPeriod);
    }
}