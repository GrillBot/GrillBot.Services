using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request.CreateItems;

public class MessageEditedRequest
{
    [Required]
    public string JumpUrl { get; set; } = null!;
    
    public string? ContentBefore { get; set; }
    
    [Required]
    public string ContentAfter { get; set; } = null!;
}
