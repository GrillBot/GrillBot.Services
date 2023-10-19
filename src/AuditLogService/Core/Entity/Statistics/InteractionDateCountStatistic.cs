namespace AuditLogService.Core.Entity.Statistics;

public class InteractionDateCountStatistic : DateCountStatistic
{
    public InteractionDateCountStatistic()
    {
    }

    public InteractionDateCountStatistic(DateOnly date) : base(date)
    {
    }
}
