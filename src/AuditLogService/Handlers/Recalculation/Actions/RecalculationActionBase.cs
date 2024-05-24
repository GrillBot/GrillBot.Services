using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Events.Recalculation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuditLogService.Handlers.Recalculation.Actions;

public abstract class RecalculationActionBase
{
    protected AuditLogServiceContext DbContext { get; }
    protected AuditLogStatisticsContext StatisticsContext { get; }

    protected RecalculationActionBase(IServiceProvider serviceProvider)
    {
        DbContext = serviceProvider.GetRequiredService<AuditLogServiceContext>();
        StatisticsContext = serviceProvider.GetRequiredService<AuditLogStatisticsContext>();
    }

    public virtual bool CheckPreconditions(RecalculationPayload payload) => true;
    public abstract Task ProcessAsync(RecalculationPayload payload);

    protected async Task<TStatEntity> GetOrCreateStatEntity<TStatEntity>(Expression<Func<TStatEntity, bool>> searchExpression, params object[] primaryKeyData)
        where TStatEntity : class
    {
        var entity = await StatisticsContext.Set<TStatEntity>().FirstOrDefaultAsync(searchExpression);

        if (entity is null)
        {
            entity = (TStatEntity)Activator.CreateInstance(typeof(TStatEntity), primaryKeyData)!;
            await StatisticsContext.AddAsync(entity);
        }

        return entity;
    }
}
