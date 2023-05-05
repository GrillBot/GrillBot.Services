namespace AuditLogService.Core.Entity;

public class ChannelDeleted : ChildEntityBase
{
    public Guid ChannelInfoId { get; set; }

    public ChannelInfo ChannelInfo { get; set; } = null!;
}
