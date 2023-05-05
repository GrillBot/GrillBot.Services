using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity;

public class Unban : ChildEntityBase
{
    [StringLength(32)]
    public string UserId { get; set; } = null!;
}
