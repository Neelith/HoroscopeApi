using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Infrastructure.AI;

public sealed record GenerateHoroscopeRequest(
    DateOnly Date,
    ZodiacSignInfo SignInfo);