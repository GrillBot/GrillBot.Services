using GrillBot.Core.RabbitMQ.V2.Messages;

namespace RubbergodService.Models.Events.Pins;

public class ClearPinCachePayload : IRabbitMessage
{
    public string Topic => "Rubbergod";
    public string Queue => "ClearPinCache";

    public string GuildId { get; set; } = null!;
    public string ChannelId { get; set; } = null!;

    public ClearPinCachePayload()
    {
    }

    public ClearPinCachePayload(string guildId, string channelId)
    {
        GuildId = guildId;
        ChannelId = channelId;
    }
}
