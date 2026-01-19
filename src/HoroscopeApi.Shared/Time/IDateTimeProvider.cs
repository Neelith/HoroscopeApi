namespace HoroscopeApi.Shared.Time;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
