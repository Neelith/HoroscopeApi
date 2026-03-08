using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Domain.Horoscopes;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public interface IHoroscopePromptBuilder
{
    Task<List<ChatMessage>> BuildMessageAsync(DateOnly date, ZodiacSignInfo zodiacSignInfo, CancellationToken cancellationToken = default);
    Task<List<ChatMessage>> BuildWeeklyMessageAsync(DateOnly date, ZodiacSignInfo zodiacSignInfo, CancellationToken cancellationToken = default);
    Task<List<ChatMessage>> BuildMonthlyMessageAsync(DateOnly date, ZodiacSignInfo zodiacSignInfo, CancellationToken cancellationToken = default);
    Task<List<ChatMessage>> BuildYearlyMessageAsync(int year, ZodiacSignInfo zodiacSignInfo, CancellationToken cancellationToken = default);
}
