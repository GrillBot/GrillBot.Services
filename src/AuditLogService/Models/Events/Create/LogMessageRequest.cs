namespace AuditLogService.Models.Events.Create;

public class LogMessageRequest
{
    public string Message { get; set; } = null!;
    public string SourceAppName { get; set; } = null!;
    public string Source { get; set; } = null!;
}
