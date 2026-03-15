using HoroscopeApi.Application.Features.Horoscopes.GetWeeklyHoroscope;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.HoroscopeService;
using HoroscopeApi.Application.Services.Time;

namespace HoroscopeApi.Tests.Application.Features.Horoscopes;

public sealed class GetWeeklyHoroscopeQueryHandlerTests
{
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly Mock<IHoroscopeQueryService> _queryServiceMock = new();

    private GetWeeklyHoroscopeQueryHandler CreateHandler()
    {
        return new GetWeeklyHoroscopeQueryHandler(_queryServiceMock.Object, _dateTimeProviderMock.Object);
    }

    private static Response<HoroscopeData> BuildResponse() =>
        Response<HoroscopeData>.Create(new HoroscopeData
        {
            Sign = "cancer",
            Period = "weekly",
            Date = new DateOnly(2024, 7, 1),
            SignInfo = new ZodiacSignInfoData
            {
                Name = "Cancer",
                Symbol = "♋",
                Element = "Water",
                Quality = "Cardinal",
                Polarity = "Negative",
                RulingPlanet = "Moon",
                DateRange = "June 21 - July 22",
                Description = "The nurturer."
            },
            Predictions = new HoroscopePredictions { General = "Nurture connections." },
            LuckyNumbers = [2, 7, 11],
            LuckyColors = ["silver"],
            MoodScore = 7,
            Keywords = ["home"]
        });

    [Fact]
    public async Task Handle_WithValidSignName_ReturnsHoroscopeData()
    {
        var now = new DateTime(2024, 7, 1);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);
        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                ZodiacSign.Cancer, HoroscopePeriod.Weekly,
                DateOnly.FromDateTime(now), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(BuildResponse()));

        var result = await CreateHandler().Handle(new GetWeeklyHoroscopeQuery("Cancer"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("weekly", result.Value!.Data.Period);
    }

    [Fact]
    public async Task Handle_WithInvalidSignName_ReturnsInvalidNameError()
    {
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(DateTime.UtcNow);

        var result = await CreateHandler().Handle(new GetWeeklyHoroscopeQuery("NotValid"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_UsesPeriodWeekly()
    {
        var now = DateTime.UtcNow;
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);
        HoroscopePeriod capturedPeriod = default;

        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                It.IsAny<ZodiacSign>(), It.IsAny<HoroscopePeriod>(),
                It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .Callback<ZodiacSign, HoroscopePeriod, DateOnly, CancellationToken>((_, p, _, _) => capturedPeriod = p)
            .ReturnsAsync(Result.Ok(BuildResponse()));

        await CreateHandler().Handle(new GetWeeklyHoroscopeQuery("Aries"), CancellationToken.None);

        Assert.Equal(HoroscopePeriod.Weekly, capturedPeriod);
    }
}