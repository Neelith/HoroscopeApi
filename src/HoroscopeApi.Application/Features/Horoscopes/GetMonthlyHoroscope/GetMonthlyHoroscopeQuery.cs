using HoroscopeApi.Application.Models;

namespace HoroscopeApi.Application.Features.Horoscopes.GetMonthlyHoroscope;

public sealed record GetMonthlyHoroscopeQuery(string SignName)
    : IQuery<Response<HoroscopeData>>;