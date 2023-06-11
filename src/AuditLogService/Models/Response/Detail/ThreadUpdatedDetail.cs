using GrillBot.Core.Models;

namespace AuditLogService.Models.Response.Detail;

public class ThreadUpdatedDetail
{
    public Diff<List<string>>? Tags { get; set; } = new();
}
