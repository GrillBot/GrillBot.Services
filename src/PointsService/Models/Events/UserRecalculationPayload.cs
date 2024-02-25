
using GrillBot.Core.RabbitMQ;

namespace PointsService.Models.Events;

public class UserRecalculationPayload : IPayload
{
    public string QueueName => "points:user_recalculation";

    public string GuildId { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public UserRecalculationPayload()
    {
    }

    public UserRecalculationPayload(string guildId, string userId)
    {
        GuildId = guildId;
        UserId = userId;
    }
}
