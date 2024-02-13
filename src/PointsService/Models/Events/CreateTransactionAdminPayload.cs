namespace PointsService.Models.Events;

public class CreateTransactionAdminPayload : CreateTransactionBasePayload
{
    public const string QueueName = "points:create_transaction_requests:admin";

    public int Amount { get; set; }
}
