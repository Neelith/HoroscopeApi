using HoroscopeApi.Application.Models;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDateHoroscope;

public sealed record GetDateHoroscopeRequest(string SignName, DateOnly Date)
    : IQuery<Response<HoroscopeData>>;
