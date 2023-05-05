using System.ComponentModel.DataAnnotations;
using Discord;

namespace AuditLogService.Models.Request;

public class OverwriteInfoRequest
{
    [Required]
    public PermissionTarget Target { get; set; }

    [Required]
    [StringLength(32)]
    public string TargetId { get; set; } = null!;

    public string AllowValue { get; set; } = "0";
    public string DenyValue { get; set; } = "0";
}
