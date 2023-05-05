using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity;

public class UserLeft : ChildEntityBase
{
    [StringLength(32)]
    public string UserId { get; set; } = null!;

    public int MemberCount { get; set; }
    public bool IsBan { get; set; }
    public string? BanReason { get; set; }
}
