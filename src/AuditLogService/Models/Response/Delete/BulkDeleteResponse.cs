namespace AuditLogService.Models.Response.Delete;

public class BulkDeleteResponse
{
    public List<DeleteItemResponse> Items { get; set; } = new();
}
