using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request.CreateItems;

public class DeletedEmoteRequest
{
    [Required]
    public string EmoteId { get; set; } = null!;
}
