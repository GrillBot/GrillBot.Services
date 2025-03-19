namespace AuditLogService.Models.Events.Create;

public class CreateItemsPayload
{
    public List<LogRequest> Items { get; set; } = [];

    public CreateItemsPayload()
    {
    }

    public CreateItemsPayload(List<LogRequest> items)
    {
        Items = items;
    }
}
