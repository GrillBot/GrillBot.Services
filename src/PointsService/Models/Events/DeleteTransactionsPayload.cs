using GrillBot.Core.RabbitMQ.V2.Messages;

namespace PointsService.Models.Events;

public class DeleteTransactionsPayload : IRabbitMessage
{
    public string Topic => "Points";
    public string Queue => "DeleteTransactions";

    public string GuildId { get; set; } = null!;
    public string MessageId { get; set; } = null!;
    public string? ReactionId { get; set; }

    public DeleteTransactionsPayload()
    {
    }

    public DeleteTransactionsPayload(string guildId, string messageId, string? reactionId = null)
    {
        GuildId = guildId;
        MessageId = messageId;
        ReactionId = reactionId;
    }
}
