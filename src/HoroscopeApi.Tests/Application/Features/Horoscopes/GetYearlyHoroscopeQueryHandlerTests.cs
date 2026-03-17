using HoroscopeApi.Application.Features.Horoscopes.GetYearlyHoroscope;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.HoroscopeService;
using HoroscopeApi.Application.Services.Time;

namespace HoroscopeApi.Tests.Application.Features.Horoscopes;

public sealed class GetYearlyHoroscopeQueryHandlerTests
{
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly Mock<IHoroscopeQueryService> _queryServiceMock = new();

    private GetYearlyHoroscopeQueryHandler CreateHandler()
    {
        return new GetYearlyHoroscopeQueryHandler(_queryServiceMock.Object, _dateTimeProviderMock.Object);
    }

    private static HoroscopeData BuildHoroscopeData(int year)
    {
        return new HoroscopeData
        {
            ZodiacSign = new ZodiacSignData
            {
                Name = "Virgo",
                Symbol = "♍",
                Element = "Earth",
                Quality = "Mutable",
                Polarity = "Negative",
                RulingPlanet = "Mercury",
                DateRange = "August 23 - September 22",
                Description = "The analyst."
            },
            Period = "yearly",
            Date = new DateOnly(year, 1, 1),
            Predictions = new HoroscopePredictions { General = "Analyze carefully." },
            LuckyNumbers = [3, 5, 15],
            LuckyColors = ["green"],
            MoodScore = 7,
            Keywords = ["precision"]
        };
    }

    [Fact]
    public async Task Handle_WithExplicitYear_UsesProvidedYear()
    {
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(new DateTime(2024, 6, 1));

        DateOnly capturedDate = default;
        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                It.IsAny<ZodiacSign>(), It.IsAny<HoroscopePeriod>(),
                It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .Callback<ZodiacSign, HoroscopePeriod, DateOnly, CancellationToken>((_, _, d, _) => capturedDate = d)
            .ReturnsAsync(Result.Ok((BuildHoroscopeData(2025), false)));

        await CreateHandler().Handle(new GetYearlyHoroscopeQuery("Virgo", 2025), CancellationToken.None);

        Assert.Equal(new DateOnly(2025, 1, 1), capturedDate);
    }

    [Fact]
    public async Task Handle_WithNullYear_UsesCurrentYear()
    {
        var now = new DateTime(2024, 6, 1);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);

        DateOnly capturedDate = default;
        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                It.IsAny<ZodiacSign>(), It.IsAny<HoroscopePeriod>(),
                It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .Callback<ZodiacSign, HoroscopePeriod, DateOnly, CancellationToken>((_, _, d, _) => capturedDate = d)
            .ReturnsAsync(Result.Ok((BuildHoroscopeData(2024), false)));

        await CreateHandler().Handle(new GetYearlyHoroscopeQuery("Aries"), CancellationToken.None);

        Assert.Equal(new DateOnly(2024, 1, 1), capturedDate);
    }

    [Fact]
    public async Task Handle_WithValidSignName_ReturnsHoroscopeData()
    {
        var now = new DateTime(2024, 1, 1);
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(now);
        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                ZodiacSign.Virgo, HoroscopePeriod.Yearly,
                new DateOnly(2024, 1, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok((BuildHoroscopeData(2024), false)));

        var result = await CreateHandler().Handle(new GetYearlyHoroscopeQuery("Virgo"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("yearly", result.Value!.Data.Period);
    }

    [Fact]
    public async Task Handle_WithInvalidSignName_ReturnsInvalidNameError()
    {
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(DateTime.UtcNow);

        var result = await CreateHandler().Handle(new GetYearlyHoroscopeQuery("Zodiac"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_UsesPeriodYearly()
    {
        _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(DateTime.UtcNow);
        HoroscopePeriod capturedPeriod = default;

        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                It.IsAny<ZodiacSign>(), It.IsAny<HoroscopePeriod>(),
                It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .Callback<ZodiacSign, HoroscopePeriod, DateOnly, CancellationToken>((_, p, _, _) => capturedPeriod = p)
            .ReturnsAsync(Result.Ok((BuildHoroscopeData(2024), false)));

        await CreateHandler().Handle(new GetYearlyHoroscopeQuery("Aries"), CancellationToken.None);

        Assert.Equal(HoroscopePeriod.Yearly, capturedPeriod);
    }
}