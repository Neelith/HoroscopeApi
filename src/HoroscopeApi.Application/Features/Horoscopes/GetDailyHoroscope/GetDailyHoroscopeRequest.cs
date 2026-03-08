using HoroscopeApi.Application.Models;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;

public sealed record GetDailyHoroscopeRequest(string SignName)
    : IQuery<Response<HoroscopeData>>;
