using GrillBot.Core.RabbitMQ;

namespace RubbergodService.Models.Events.Pins;

public class ClearPinCachePayload : IPayload
{
    public string QueueName => "rubbergod:clear_pin_cache";

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
