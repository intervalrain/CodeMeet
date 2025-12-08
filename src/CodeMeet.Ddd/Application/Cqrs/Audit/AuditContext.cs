
namespace CodeMeet.Ddd.Application.Cqrs.Audit;

public class AuditContext : IAuditContext
{
    public string TraceId { get; set; } = Guid.NewGuid().ToString("N");
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }

    public DateTime RequestTime { get; } = DateTime.UtcNow;
}
