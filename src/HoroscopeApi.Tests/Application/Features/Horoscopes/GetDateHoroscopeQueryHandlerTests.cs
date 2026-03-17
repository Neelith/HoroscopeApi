using HoroscopeApi.Application.Features.Horoscopes.GetDateHoroscope;
using HoroscopeApi.Application.Models;
using HoroscopeApi.Application.Services.HoroscopeService;

namespace HoroscopeApi.Tests.Application.Features.Horoscopes;

public sealed class GetDateHoroscopeQueryHandlerTests
{
    private readonly Mock<IHoroscopeQueryService> _queryServiceMock = new();

    private GetDateHoroscopeQueryHandler CreateHandler()
    {
        return new GetDateHoroscopeQueryHandler(_queryServiceMock.Object);
    }

    private static HoroscopeData BuildHoroscopeData(DateOnly date)
    {
        return new HoroscopeData
        {
            ZodiacSign = new ZodiacSignData
            {
                Name = "Libra",
                Symbol = "♎",
                Element = "Air",
                Quality = "Cardinal",
                Polarity = "Positive",
                RulingPlanet = "Venus",
                DateRange = "September 23 - October 22",
                Description = "The balanced sign."
            },
            Period = "daily",
            Date = date,
            Predictions = new HoroscopePredictions { General = "Balance is key." },
            LuckyNumbers = [2, 6, 9],
            LuckyColors = ["blue"],
            MoodScore = 7,
            Keywords = ["balance"]
        };
    }

    [Fact]
    public async Task Handle_WithValidSignAndDate_ReturnsHoroscopeData()
    {
        var date = new DateOnly(2024, 10, 1);
        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                ZodiacSign.Libra, HoroscopePeriod.Daily, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok((BuildHoroscopeData(date), false)));

        var result = await CreateHandler().Handle(new GetDateHoroscopeQuery("Libra", date), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Libra", result.Value!.Data.ZodiacSign.Name);
        Assert.Equal(date, result.Value.Data.Date);
    }

    [Fact]
    public async Task Handle_WithInvalidSignName_ReturnsInvalidNameError()
    {
        var date = new DateOnly(2024, 10, 1);

        var result = await CreateHandler()
            .Handle(new GetDateHoroscopeQuery("InvalidSign", date), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("ZodiacSign.InvalidName", result.Errors[0].Code);
    }

    [Fact]
    public async Task Handle_PassesProvidedDateToService()
    {
        var date = new DateOnly(2023, 5, 20);
        DateOnly capturedDate = default;

        _queryServiceMock.Setup(s => s.GetOrGenerateHoroscopeAsync(
                It.IsAny<ZodiacSign>(), It.IsAny<HoroscopePeriod>(),
                It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .Callback<ZodiacSign, HoroscopePeriod, DateOnly, CancellationToken>((_, _, d, _) => capturedDate = d)
            .ReturnsAsync(Result.Ok((BuildHoroscopeData(date), false)));

        await CreateHandler().Handle(new GetDateHoroscopeQuery("Aries", date), CancellationToken.None);

        Assert.Equal(date, capturedDate);
    }
}