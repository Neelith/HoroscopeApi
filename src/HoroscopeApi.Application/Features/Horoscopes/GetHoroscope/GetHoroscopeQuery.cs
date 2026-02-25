using System;
using Hermes.Requests;
using Hermes.Responses;
using HoroscopeApi.Application.Features.Shared;
using HoroscopeApi.Domain.Horoscopes;

namespace HoroscopeApi.Application.Features.Horoscopes.GetHoroscope;

public sealed record GetHoroscopeQuery(
    string SignName, 
    HoroscopePeriod? Period = null, 
    DateOnly? Date = null) 
    : IQuery<Response<HoroscopeData>>;
