namespace AuditLogService.Core.Entity.Statistics;

public class InteractionUserActionStatistic : UserActionStatBase
{
    public InteractionUserActionStatistic()
    {
    }

    public InteractionUserActionStatistic(string action, string userId) : base(action, userId)
    {
    }
}
