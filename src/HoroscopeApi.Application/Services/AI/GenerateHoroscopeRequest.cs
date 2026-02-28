using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Services.AI;

public sealed record GenerateHoroscopeRequest(
    DateOnly Date,
    ZodiacSignInfo SignInfo);