using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request;

public class UnbanRequest
{
    [Required]
    [StringLength(32)]
    public string UserId { get; set; } = null!;
}
