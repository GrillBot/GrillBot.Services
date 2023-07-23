using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity.Statistics;

public class DailyAvgTimes
{
    [Key]
    public DateOnly Date { get; set; }

    public double Interactions { get; set; } = -1;
    public double Jobs { get; set; } = -1;
    public double ExternalApi { get; set; } = -1;
    public double InternalApi { get; set; } = -1;

    public DailyAvgTimes()
    {
    }

    public DailyAvgTimes(DateOnly date)
    {
        Date = date;
    }
}
