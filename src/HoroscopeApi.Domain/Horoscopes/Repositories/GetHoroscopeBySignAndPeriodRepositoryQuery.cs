namespace HoroscopeApi.Domain.Horoscopes.Repositories;

public sealed record GetHoroscopeBySignAndPeriodRepositoryQuery(
    int ZodiacSignId,
    HoroscopePeriod Period,
    DateOnly Date);
