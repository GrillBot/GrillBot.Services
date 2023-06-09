using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request.CreateItems;

public class DeletedEmoteRequest
{
    [Required]
    [StringLength(32)]
    public string EmoteId { get; set; } = null!;
}
