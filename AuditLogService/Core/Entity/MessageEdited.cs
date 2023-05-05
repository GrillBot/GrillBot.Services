namespace AuditLogService.Core.Entity;

public class MessageEdited : ChildEntityBase
{
    public string JumpUrl { get; set; } = null!;
    public string ContentBefore { get; set; } = null!;
    public string ContentAfter { get; set; } = null!;
}
