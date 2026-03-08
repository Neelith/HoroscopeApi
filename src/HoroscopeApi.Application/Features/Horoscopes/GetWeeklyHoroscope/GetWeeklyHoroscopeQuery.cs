using HoroscopeApi.Application.Models;

namespace HoroscopeApi.Application.Features.Horoscopes.GetWeeklyHoroscope;

public sealed record GetWeeklyHoroscopeQuery(string SignName)
    : IQuery<Response<HoroscopeData>>;