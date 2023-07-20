using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity.Statistics;

public class UserActionStatBase
{
    [StringLength(128)]
    public string Action { get; set; } = null!;

    [StringLength(255)]
    public string UserId { get; set; } = null!;

    public long Count { get; set; }
}
