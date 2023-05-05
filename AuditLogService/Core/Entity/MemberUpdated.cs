namespace AuditLogService.Core.Entity;

public class MemberUpdated : ChildEntityBase
{
    public Guid BeforeId { get; set; }
    public Guid AfterId { get; set; }

    public MemberInfo Before { get; set; } = null!;
    public MemberInfo After { get; set; } = null!;
}
