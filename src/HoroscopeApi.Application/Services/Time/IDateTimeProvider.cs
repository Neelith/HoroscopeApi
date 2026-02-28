namespace HoroscopeApi.Application.Services.Time;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}