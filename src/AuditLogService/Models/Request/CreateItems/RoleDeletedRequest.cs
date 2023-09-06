using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request.CreateItems;

public class RoleDeletedRequest
{
    [Required]
    [StringLength(32)]
    public string RoleId { get; set; } = null!;
}
