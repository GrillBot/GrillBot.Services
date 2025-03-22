using GrillBot.Core.RabbitMQ.V2.Messages;

namespace PointsService.Models.Events;

public abstract class CreateTransactionBasePayload : IRabbitMessage
{
    public string Topic => "Points";
    public abstract string Queue { get; }

    public string GuildId { get; set; } = null!;

    protected CreateTransactionBasePayload()
    {
    }

    protected CreateTransactionBasePayload(string guildId)
    {
        GuildId = guildId;
    }
}
