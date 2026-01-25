using Hermes.Results;
using HoroscopeApi.Domain.Horoscopes;

namespace HoroscopeApi.Application.Infrastructure.AI;

public interface IHoroscopeGeneratorService
{
    Task<Result<List<Horoscope>>> GenerateDailyHoroscopesAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);
}
