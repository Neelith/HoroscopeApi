using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Domain.Horoscopes.Repositories;

public sealed record GetHoroscopesByDateRangeRepositoryQuery(
    ZodiacSign Sign,
    HoroscopePeriod Period,
    DateOnly StartDate,
    DateOnly EndDate);
