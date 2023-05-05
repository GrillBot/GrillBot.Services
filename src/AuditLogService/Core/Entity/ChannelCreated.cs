namespace AuditLogService.Core.Entity;

public class ChannelCreated : ChildEntityBase
{
    public Guid ChannelInfoId { get; set; }

    public ChannelInfo ChannelInfo { get; set; } = null!;
}
