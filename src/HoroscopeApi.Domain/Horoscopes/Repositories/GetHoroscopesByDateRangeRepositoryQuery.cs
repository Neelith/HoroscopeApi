namespace HoroscopeApi.Domain.Horoscopes.Repositories;

public sealed record GetHoroscopesByDateRangeRepositoryQuery(
    int ZodiacSignId,
    HoroscopePeriod Period,
    DateOnly StartDate,
    DateOnly EndDate);
