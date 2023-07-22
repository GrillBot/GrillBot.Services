using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

    protected async Task<TStatisticEntity> GetOrCreateStatisticEntity<TStatisticEntity>(Expression<Func<TStatisticEntity, bool>> searchExpression) where TStatisticEntity : class
    {
        var stats = await StatisticsContext.Set<TStatisticEntity>()
            .FirstOrDefaultAsync(searchExpression);

        if (stats is null)
        {
            stats = Activator.CreateInstance<TStatisticEntity>();
            await StatisticsContext.AddAsync(stats);
        }

        return stats;
    }
}
