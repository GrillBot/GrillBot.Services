namespace AuditLogService.Models.Events.Create;

public class MessageEditedRequest
{
    public string JumpUrl { get; set; } = null!;
    public string? ContentBefore { get; set; }
    public string ContentAfter { get; set; } = null!;
}
