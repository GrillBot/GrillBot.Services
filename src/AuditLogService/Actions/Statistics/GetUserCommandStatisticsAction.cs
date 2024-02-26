using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetUserCommandStatisticsAction : ApiAction
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetUserCommandStatisticsAction(AuditLogStatisticsContext statisticsContext, ICounterManager counterManager) : base(counterManager)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        List<InteractionUserActionStatistic> data;
        using (CreateCounter("Database"))
            data = await StatisticsContext.InteractionUserActionStatistics.AsNoTracking().ToListAsync();

        var result = data.ConvertAll(o => new UserActionCountItem
        {
            Action = o.Action,
            Count = o.Count,
            UserId = o.UserId!
        });

        return ApiResult.Ok(result);
    }
}
