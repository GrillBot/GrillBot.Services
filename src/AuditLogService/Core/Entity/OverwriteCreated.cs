namespace AuditLogService.Core.Entity;

public class OverwriteCreated : ChildEntityBase
{
    public Guid OverwriteInfoId { get; set; }

    public OverwriteInfo OverwriteInfo { get; set; } = null!;
}
