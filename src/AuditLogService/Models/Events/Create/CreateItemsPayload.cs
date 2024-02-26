using GrillBot.Core.RabbitMQ;

namespace AuditLogService.Models.Events.Create;

public class CreateItemsPayload : IPayload
{
    public string QueueName => "audit:create_items";

    public List<LogRequest> Items { get; set; } = new();

    public CreateItemsPayload()
    {
    }

    public CreateItemsPayload(List<LogRequest> items)
    {
        Items = items;
    }
}
