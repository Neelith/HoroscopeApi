using System.Text.Json.Serialization;

namespace HoroscopeApi.Domain.Shared;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    [JsonIgnore] public IReadOnlyList<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}