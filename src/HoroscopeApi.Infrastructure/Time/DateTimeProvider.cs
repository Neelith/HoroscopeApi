using HoroscopeApi.Shared.Time;

namespace HoroscopeApi.Infrastructure.Time;

internal class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow.ToUniversalTime();
}
