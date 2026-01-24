using System;
using Hermes.Requests;
using Hermes.Responses;
using HoroscopeApi.Application.Features.Shared;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;

public sealed record GetDailyHoroscopeQuery(string SignName, DateOnly? Date = null) 
    : IQuery<Response<HoroscopeData>>;
