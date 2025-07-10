using GrillBot.Core.Database.ValueObjects;

namespace UnverifyService.Core.Entity.Logs;

public class UnverifyLogSetChannel
{
    public Guid LogItemId { get; set; }
    public UnverifyLogSetOperation Operation { get; set; } = default!;

    public DiscordIdValueObject ChannelId { get; set; }
    public ulong AllowValue { get; set; }
    public ulong DenyValue { get; set; }
    public bool IsKept { get; set; }
}
