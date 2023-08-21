using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity.Statistics;

public class JobInfo
{
    [Key]
    [StringLength(128)]
    public string Name { get; set; } = null!;

    public int StartCount { get; set; }
    public int LastRunDuration { get; set; }
    public DateTime LastStartAt { get; set; }
    public int FailedCount { get; set; }
    public int TotalDuration { get; set; }
    public int MinTime { get; set; }
    public int MaxTime { get; set; }
    public int AvgTime { get; set; }

    public JobInfo()
    {
    }

    public JobInfo(string name)
    {
        Name = name;
    }
}
