using GrillBot.Core.RabbitMQ.V2.Messages;

namespace AuditLogService.Models.Events.Create;

public class CreateItemsMessage : IRabbitMessage
{
    public string Topic => "AuditLog";
    public string Queue => "CreateItems";

    public List<LogRequest> Items { get; set; } = [];

    public CreateItemsMessage()
    {
    }

    public CreateItemsMessage(List<LogRequest> items)
    {
        Items = items;
    }
}
