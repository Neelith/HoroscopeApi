using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Domain.Horoscopes.Repositories;

public sealed record GetHoroscopeBySignAndPeriodRepositoryQuery(
    ZodiacSign Sign,
    HoroscopePeriod Period,
    DateOnly Date);