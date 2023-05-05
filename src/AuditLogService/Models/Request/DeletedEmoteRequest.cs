using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request;

public class DeletedEmoteRequest
{
    [Required]
    [StringLength(32)]
    public string EmoteId { get; set; } = null!;

    [Required]
    public string EmoteName { get; set; } = null!;
}
