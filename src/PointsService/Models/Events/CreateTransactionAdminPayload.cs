namespace PointsService.Models.Events;

public class CreateTransactionAdminPayload : CreateTransactionBasePayload
{
    public override string QueueName => "points:create_transaction_requests:admin";

    public string UserId { get; set; } = null!;
    public int Amount { get; set; }

    public CreateTransactionAdminPayload()
    {
    }

    public CreateTransactionAdminPayload(string guildId, string userId, int amount) : base(guildId)
    {
        UserId = userId;
        Amount = amount;
    }
}
