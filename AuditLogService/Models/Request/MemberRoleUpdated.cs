using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request;

public class MemberRoleUpdated
{
    [Required]
    [StringLength(32)]
    public string UserId { get; set; } = null!;

    [Required]
    [StringLength(32)]
    public string RoleId { get; set; } = null!;

    [Required]
    public string RoleName { get; set; } = null!;

    [Required]
    [StringLength(32)]
    public string RoleColor { get; set; } = null!;

    [Required]
    public bool IsAdded { get; set; }
}
