using HoroscopeApi.Application.Models;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;

public sealed record GetDailyHoroscopeQuery(string SignName)
    : IQuery<Response<HoroscopeData>>;