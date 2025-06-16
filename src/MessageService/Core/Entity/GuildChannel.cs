using GrillBot.Core.Database.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Core.Entity;

[PrimaryKey(nameof(GuildId), nameof(ChannelId))]
public class GuildChannel
{
    public DiscordIdValueObject GuildId { get; set; }
    public DiscordIdValueObject ChannelId { get; set; }

    public bool IsAutoReplyDisabled { get; set; }
    public bool IsPointsDisabled { get; set; }
    public bool IsDeleted { get; set; }
}
