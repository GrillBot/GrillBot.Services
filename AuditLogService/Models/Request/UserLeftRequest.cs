using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request;

public class UserLeftRequest : UserJoinedRequest
{
    [Required]
    [StringLength(32)]
    public string UserId { get; set; } = null!;

    [Required]
    public bool IsBan { get; set; }
    public string? BanReason { get; set; }
}
