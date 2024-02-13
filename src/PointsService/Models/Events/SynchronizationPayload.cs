using PointsService.Models.Users;

namespace PointsService.Models.Events;

public class SynchronizationPayload
{
    public const string QueueName = "points:synchronization";

    public string GuildId { get; set; } = null!;
    public List<ChannelInfo> Channels { get; set; } = new();
    public List<UserInfo> Users { get; set; } = new();
}
