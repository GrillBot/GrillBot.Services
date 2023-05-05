using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request;

public class MessageEditedRequest
{
    [Required]
    public string JumpUrl { get; set; } = null!;
    
    [Required]
    public string ContentBefore { get; set; } = null!;
    
    [Required]
    public string ContentAfter { get; set; } = null!;
}
