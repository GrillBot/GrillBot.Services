using GrillBot.Core.RabbitMQ.V2.Messages;
using PointsService.Models.Channels;
using PointsService.Models.Users;

namespace PointsService.Models.Events;

public class SynchronizationPayload : IRabbitMessage
{
    public string Topic => "Points";
    public string Queue => "Synchronization";

    public string GuildId { get; set; } = null!;
    public List<ChannelSyncItem> Channels { get; set; } = [];
    public List<UserSyncItem> Users { get; set; } = [];

    public SynchronizationPayload()
    {
    }

    public SynchronizationPayload(string guildId, List<ChannelSyncItem> channels, List<UserSyncItem> users)
    {
        GuildId = guildId;
        Channels = channels;
        Users = users;
    }
}
