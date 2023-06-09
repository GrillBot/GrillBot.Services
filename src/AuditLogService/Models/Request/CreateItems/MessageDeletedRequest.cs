using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request.CreateItems;

public class MessageDeletedRequest
{
    [Required]
    [StringLength(32)]
    public string AuthorId { get; set; } = null!;

    [Required]
    public DateTime MessageCreatedAt { get; set; }

    public string? Content { get; set; }

    public List<EmbedRequest> Embeds { get; set; } = new();
}
