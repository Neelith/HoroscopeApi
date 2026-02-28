using HoroscopeApi.Application.Models;
using HoroscopeApi.Domain.Horoscopes;

namespace HoroscopeApi.Application.Features.Horoscopes.GetHoroscope;

public sealed record GetHoroscopeQuery(
    string SignName,
    HoroscopePeriod? Period = null,
    DateOnly? Date = null)
    : IQuery<Response<HoroscopeData>>;