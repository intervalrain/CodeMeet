namespace CodeMeet.Ddd.Domain;

public abstract record DomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}