using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Application.Infrastructure.AI;

public sealed record GenerateBatchHoroscopesRequest(
    DateOnly Date,
    IReadOnlyCollection<ZodiacSignInfo> ZodiacSigns);
