using HoroscopeApi.Application.Models;

namespace HoroscopeApi.Application.Features.Horoscopes.GetWeeklyHoroscope;

public sealed record GetWeeklyHoroscopeRequest(string SignName)
    : IQuery<Response<HoroscopeData>>;
