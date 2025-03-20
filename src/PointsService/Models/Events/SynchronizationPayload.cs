using PointsService.Models.Channels;
using PointsService.Models.Users;

namespace PointsService.Models.Events;

public class SynchronizationPayload
{
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
