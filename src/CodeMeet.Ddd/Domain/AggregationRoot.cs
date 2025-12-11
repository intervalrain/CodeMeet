namespace CodeMeet.Ddd.Domain;

/// <summary>
/// Base class for aggregate roots. Aggregates are consistency boundaries
/// and the only entry point for modifying a cluster of entities.
/// </summary>
/// <typeparam name="TId">The type of aggregate root identifier.</typeparam>
public abstract class AggregationRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregationRoot() { }

    protected AggregationRoot(TId id) : base(id) { }

    protected void AddDomainEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    public IReadOnlyList<IDomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();
        _domainEvents.Clear();
        return copy;
    }
}

/// <summary>
/// Aggregate root with Guid identifier.
/// </summary>
public abstract class AggregationRoot : AggregationRoot<Guid>
{
    protected AggregationRoot() : base(Guid.NewGuid()) { }
}