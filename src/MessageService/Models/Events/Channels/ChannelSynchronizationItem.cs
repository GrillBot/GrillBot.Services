using Discord;

namespace MessageService.Models.Events.Channels;

public class ChannelSynchronizationItem
{
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? IsPointsDisabled { get; set; }
    public bool? IsAutoReplyDisabled { get; set; }

    public ChannelSynchronizationItem()
    {
    }

    public ChannelSynchronizationItem(ulong guildId, ulong channelId)
    {
        ChannelId = channelId;
        GuildId = guildId;
    }

    public static ChannelSynchronizationItem FromChannel(IGuildChannel channel)
        => new(channel.GuildId, channel.Id);
}
