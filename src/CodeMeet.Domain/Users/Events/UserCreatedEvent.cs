using CodeMeet.Ddd.Domain;

namespace CodeMeet.Domain.Users.Events;

public record UserCreatedEvent(string Username, string Email) : DomainEvent;