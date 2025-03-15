namespace AuditLogService.Models.Response.Search;

public class MessageDeletedPreview
{
    public string AuthorId { get; set; } = null!;
    public DateTime MessageCreatedAt { get; set; }
    public int ContentLength { get; set; }
    public int EmbedCount { get; set; }
    public int EmbedFieldsCount { get; set; }
}
