namespace AuditLogService.Core.Entity;

public class OverwriteDeleted : ChildEntityBase
{
    public Guid OverwriteInfoId { get; set; }

    public OverwriteInfo OverwriteInfo { get; set; } = null!;
}
