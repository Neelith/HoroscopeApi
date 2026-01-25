using HoroscopeApi.Application.Infrastructure.AI;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public interface IHoroscopePromptBuilder
{
    List<ChatMessage> BuildMessages(DateOnly date, IReadOnlyCollection<ZodiacSignInfo> zodiacSigns);
}
