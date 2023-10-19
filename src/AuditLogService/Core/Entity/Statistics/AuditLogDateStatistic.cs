namespace AuditLogService.Core.Entity.Statistics;

public class AuditLogDateStatistic : DateCountStatistic
{
    public AuditLogDateStatistic()
    {
    }

    public AuditLogDateStatistic(DateOnly date) : base(date)
    {
    }
}
