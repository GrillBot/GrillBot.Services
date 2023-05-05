using Discord;

namespace AuditLogService.Core.Entity;

public class LogMessage : ChildEntityBase
{
    public string Message { get; set; } = null!;
    public LogSeverity Severity { get; set; }
}
