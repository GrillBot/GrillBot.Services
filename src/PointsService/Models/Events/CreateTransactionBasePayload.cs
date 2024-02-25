using GrillBot.Core.RabbitMQ;

namespace PointsService.Models.Events;

public abstract class CreateTransactionBasePayload : IPayload
{
    public abstract string QueueName { get; }

    public string GuildId { get; set; } = null!;

    protected CreateTransactionBasePayload()
    {
    }

    protected CreateTransactionBasePayload(string guildId)
    {
        GuildId = guildId;
    }
}
