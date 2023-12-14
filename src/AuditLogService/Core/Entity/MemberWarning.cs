using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity;

public class MemberWarning : ChildEntityBase
{
    public string Reason { get; set; } = null!;

    [StringLength(32)]
    public string TargetId { get; set; } = null!;
}
