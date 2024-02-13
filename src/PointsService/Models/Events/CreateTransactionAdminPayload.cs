namespace PointsService.Models.Events;

public class CreateTransactionAdminPayload : CreateTransactionBasePayload
{
    public const string QueueName = "points:create_transaction_requests:admin";

    public string UserId { get; set; } = null!;
    public int Amount { get; set; }
}
