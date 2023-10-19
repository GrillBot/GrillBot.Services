namespace AuditLogService.Core.Entity.Statistics;

public class DateCountStatistic
{
    public DateOnly Date { get; set; }
    public long Count { get; set; }

    public DateCountStatistic()
    {
    }

    public DateCountStatistic(DateOnly date)
    {
        Date = date;
    }
}
