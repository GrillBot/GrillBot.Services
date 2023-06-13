namespace AuditLogService.Core.Entity;

public class ChannelUpdated : ChildEntityBase
{
    public Guid BeforeId { get; set; }
    public Guid AfterId { get; set; }
    
    public ChannelInfo Before { get; set; } = null!;
    public ChannelInfo After { get; set; } = null!;
}
