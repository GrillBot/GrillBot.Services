namespace AuditLogService.Core.Entity;

public class OverwriteUpdated : ChildEntityBase
{
    public Guid BeforeId { get; set; }
    public Guid AfterId { get; set; }

    public OverwriteInfo Before { get; set; } = null!;
    public OverwriteInfo After { get; set; } = null!;
}
