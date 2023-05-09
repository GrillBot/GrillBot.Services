using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request;

public class MessageDeletedRequest
{
    [Required]
    [StringLength(32)]
    public string AuthorId { get; set; } = null!;

    [Required]
    public DateTime MessageCreatedAt { get; set; }

    public string? Content { get; set; }

    public List<string> Embeds { get; set; } = new();
}
