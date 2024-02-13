namespace PointsService.Models.Events;

public class DeleteTransactionsPayload
{
    public const string QueueName = "points:delete_transactions";

    public string GuildId { get; set; } = null!;
    public string MessageId { get; set; } = null!;
    public string? ReactionId { get; set; }
}
