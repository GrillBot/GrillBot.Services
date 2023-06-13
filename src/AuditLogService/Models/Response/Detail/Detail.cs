using AuditLogService.Core.Enums;

namespace AuditLogService.Models.Response.Detail;

public class Detail
{
    public LogType Type { get; set; }
    public object? Data { get; set; }
}
