namespace PointsService.Models.Events;

public class DeleteTransactionsPayload
{
    public const string QueueName = "points:delete_transactions";

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
