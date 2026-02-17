using HoroscopeApi.Application.Infrastructure.AI;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public interface IHoroscopePromptBuilder
{
    List<ChatMessage> BuildMessage(DateOnly date, ZodiacSignInfo zodiacSignInfo);
    List<ChatMessage> BuildYearlyMessage(int year, ZodiacSignInfo zodiacSignInfo);
}
