namespace AuditLogService.Models.Events.Create;

public class MessageDeletedRequest
{
    public string AuthorId { get; set; } = null!;
    public DateTime MessageCreatedAt { get; set; }
    public string? Content { get; set; }
    public List<EmbedRequest> Embeds { get; set; } = [];
}
