namespace AuditLogService.Models.Events;

public class BulkDeletePayload
{
    public List<Guid> Ids { get; set; } = [];

    public BulkDeletePayload()
    {
    }

    public BulkDeletePayload(List<Guid> ids)
    {
        Ids = ids;
    }
}
