using System.ComponentModel.DataAnnotations;
using Discord;

namespace AuditLogService.Models.Request.CreateItems;

public class LogMessageRequest
{
    [Required]
    public string Message { get; set; } = null!;

    [Required]
    public LogSeverity Severity { get; set; }

    [Required]
    [StringLength(100)]
    public string SourceAppName { get; set; } = null!;

    [Required]
    [StringLength(512)]
    public string Source { get; set; } = null!;
}
