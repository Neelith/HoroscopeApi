namespace HoroscopeApi.Shared.Domain;

public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    Task Handle(T domainEvent, CancellationToken? cancellationToken = default);
}