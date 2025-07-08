using GrillBot.Core.Database.ValueObjects;

namespace UnverifyService.Core.Entity.Logs;

public class UnverifyLogRemoveChannel
{
    public Guid LogItemId { get; set; }
    public UnverifyLogRemoveOperation Operation { get; set; } = default!;

    public DiscordIdValueObject ChannelId { get; set; }
    public ulong AllowValue { get; set; }
    public ulong DenyValue { get; set; }
}
