using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request.CreateItems;

public class UnbanRequest
{
    [Required]
    [StringLength(32)]
    public string UserId { get; set; } = null!;
}
