using GrillBot.Core.RabbitMQ.V2.Messages;

namespace PointsService.Models.Events;

public class UserRecalculationPayload : IRabbitMessage
{
    public string Topic => "Points";
    public string Queue => "UserRecalculation";

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
