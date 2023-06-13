namespace AuditLogService.Core.Entity;

public class ThreadDeleted : ChildEntityBase
{
    public Guid ThreadInfoId { get; set; }

    public ThreadInfo ThreadInfo { get; set; } = null!;
}
