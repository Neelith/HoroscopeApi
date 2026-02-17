using System;
using Hermes.Requests;
using Hermes.Responses;
using HoroscopeApi.Application.Features.Shared;

namespace HoroscopeApi.Application.Features.Horoscopes.GetYearlyHoroscope;

public sealed record GetYearlyHoroscopeQuery(string SignName, int? Year = null) 
    : IQuery<Response<HoroscopeData>>;
