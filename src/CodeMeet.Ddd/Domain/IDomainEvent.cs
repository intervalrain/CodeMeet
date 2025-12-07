namespace CodeMeet.Ddd.Domain;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent something that happended in the domain.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// The unique identifier for this event instance.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// When the event occurred.
    /// </summary>
    DateTime OccurredAt { get; }
}