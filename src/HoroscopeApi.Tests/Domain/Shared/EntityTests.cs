using HoroscopeApi.Domain.Shared;

namespace HoroscopeApi.Tests.Domain.Shared;

file sealed class TestDomainEvent : IDomainEvent;

file sealed class TestEntity : Entity;

public sealed class EntityTests
{
    [Fact]
    public void DomainEvents_InitiallyEmpty()
    {
        var entity = new TestEntity();
        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public void Raise_AddsDomainEvent()
    {
        var entity = new TestEntity();
        var domainEvent = new TestDomainEvent();

        entity.Raise(domainEvent);

        Assert.Single(entity.DomainEvents);
        Assert.Same(domainEvent, entity.DomainEvents[0]);
    }

    [Fact]
    public void Raise_MultipleEvents_AddsAll()
    {
        var entity = new TestEntity();

        entity.Raise(new TestDomainEvent());
        entity.Raise(new TestDomainEvent());
        entity.Raise(new TestDomainEvent());

        Assert.Equal(3, entity.DomainEvents.Count);
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        var entity = new TestEntity();
        entity.Raise(new TestDomainEvent());
        entity.Raise(new TestDomainEvent());

        entity.ClearDomainEvents();

        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public void ClearDomainEvents_OnEmptyCollection_DoesNotThrow()
    {
        var entity = new TestEntity();
        var ex = Record.Exception(() => entity.ClearDomainEvents());
        Assert.Null(ex);
    }
}
