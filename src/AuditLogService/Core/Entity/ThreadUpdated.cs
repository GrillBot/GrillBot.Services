namespace AuditLogService.Core.Entity;

public class ThreadUpdated : ChildEntityBase
{
    public Guid BeforeId { get; set; }
    public Guid AfterId { get; set; }

    public ThreadInfo Before { get; set; } = null!;
    public ThreadInfo After { get; set; } = null!;
}
