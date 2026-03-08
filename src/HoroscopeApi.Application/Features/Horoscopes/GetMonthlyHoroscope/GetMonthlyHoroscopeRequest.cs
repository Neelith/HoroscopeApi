using HoroscopeApi.Application.Models;

namespace HoroscopeApi.Application.Features.Horoscopes.GetMonthlyHoroscope;

public sealed record GetMonthlyHoroscopeRequest(string SignName)
    : IQuery<Response<HoroscopeData>>;
