using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity.Statistics;

public class InteractionStatistic
{
    [Key]
    [StringLength(4096)]
    public string Action { get; set; } = null!;

    public DateTime LastRun { get; set; }
    public long FailedCount { get; set; }
    public long MaxDuration { get; set; }
    public long MinDuration { get; set; }
    public long SuccessCount { get; set; }
    public long TotalDuration { get; set; }
    public long LastRunDuration { get; set; }

    public InteractionStatistic()
    {
    }

    public InteractionStatistic(string action)
    {
        Action = action;
    }
}
