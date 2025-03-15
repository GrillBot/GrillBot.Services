namespace AuditLogService.Models.Response.Search;

public class MessageEditedPreview
{
    public int ContentLengthBefore { get; set; }
    public int ContentLengthAfter { get; set; }
    public string JumpUrl { get; set; } = null!;
}
