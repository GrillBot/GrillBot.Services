using System.ComponentModel.DataAnnotations;
using Discord;

namespace AuditLogService.Core.Entity;

public class LogMessage : ChildEntityBase
{
    public string Message { get; set; } = null!;
    public LogSeverity Severity { get; set; }

    [StringLength(100)]
    public string SourceAppName { get; set; } = null!;

    [StringLength(512)]
    public string Source { get; set; } = null!;
}
