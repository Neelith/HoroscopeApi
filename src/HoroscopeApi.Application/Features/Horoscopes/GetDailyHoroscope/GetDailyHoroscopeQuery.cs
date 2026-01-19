using System;
using Hermes.Requests;
using HoroscopeApi.Application.Features.Shared;

namespace HoroscopeApi.Application.Features.Horoscopes.GetDailyHoroscope;

public sealed record GetDailyHoroscopeQuery(string SignName, DateOnly? Date = null) 
    : IQuery<HoroscopeResponse>;
