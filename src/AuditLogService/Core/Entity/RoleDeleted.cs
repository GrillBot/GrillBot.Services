namespace AuditLogService.Core.Entity;

public class RoleDeleted : ChildEntityBase
{
    public Guid RoleInfoId { get; set; }

    public RoleInfo RoleInfo { get; set; } = null!;
}
