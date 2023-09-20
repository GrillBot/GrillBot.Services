using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Core.Entity;

[Index(nameof(JobName))]
[Index(nameof(EndAt))]
public class JobExecution : ChildEntityBase
{
    [StringLength(128)]
    public string JobName { get; set; } = null!;

    public string Result { get; set; } = null!;

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public bool WasError { get; set; }

    [StringLength(32)]
    public string? StartUserId { get; set; }

    public long Duration { get; set; }
}
