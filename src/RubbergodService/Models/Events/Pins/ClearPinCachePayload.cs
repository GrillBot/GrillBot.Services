namespace RubbergodService.Models.Events.Pins;

public class ClearPinCachePayload
{
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
