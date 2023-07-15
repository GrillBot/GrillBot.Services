using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;

namespace AuditLogService.BackgroundServices.Actions;

public abstract class PostProcessActionBase
{
    protected AuditLogServiceContext Context { get; }
    protected AuditLogStatisticsContext StatisticsContext { get; }

    protected PostProcessActionBase(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext)
    {
        Context = context;
        StatisticsContext = statisticsContext;
    }

    public abstract bool CanProcess(LogItem logItem);
    public abstract Task ProcessAsync(LogItem logItem);
}
