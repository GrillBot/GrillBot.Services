using Discord;
using GrillBot.Core.RabbitMQ.V2.Messages;

namespace EmoteService.Models.Events.Guild;

public class GuildChannelDeletedPayload : IRabbitMessage
{
    public string Topic => "Emote";
    public string Queue => "GuildChannelDeleted";

    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }

    public GuildChannelDeletedPayload()
    {
    }

    public GuildChannelDeletedPayload(ulong guildId, ulong channelId)
    {
        GuildId = guildId;
        ChannelId = channelId;
    }

    public static GuildChannelDeletedPayload Create(IGuild guild, IChannel channel)
        => new(guild.Id, channel.Id);

    public static GuildChannelDeletedPayload Create(IGuildChannel channel)
        => new(channel.GuildId, channel.Id);
}
