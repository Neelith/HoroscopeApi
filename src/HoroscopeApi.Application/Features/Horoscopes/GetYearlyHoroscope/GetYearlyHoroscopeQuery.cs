using HoroscopeApi.Application.Models;

namespace HoroscopeApi.Application.Features.Horoscopes.GetYearlyHoroscope;

public sealed record GetYearlyHoroscopeQuery(string SignName, int? Year = null)
    : IQuery<Response<HoroscopeData>>;