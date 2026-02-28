namespace HoroscopeApi.Domain.Shared;

public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    Task Handle(T domainEvent, CancellationToken? cancellationToken = default);
}