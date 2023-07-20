using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetUserApiStatisticsAction : ApiActionBase
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetUserApiStatisticsAction(AuditLogStatisticsContext statisticsContext)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var criteria = (string)Parameters[0]!;

        var data = await Filter(StatisticsContext.ApiUserActionStatistics.AsNoTracking(), criteria)
            .Select(o => new { o.Action, o.Count, o.UserId })
            .ToListAsync();

        var result = data.ConvertAll(o => new UserActionCountItem
        {
            Action = o.Action,
            Count = o.Count,
            UserId = o.UserId
        });

        return ApiResult.FromSuccess(result);
    }

    private static IQueryable<ApiUserActionStatistic> Filter(IQueryable<ApiUserActionStatistic> query, string criteria)
    {
        return criteria switch
        {
            "v1-private" => query.Where(o => o.ApiGroup == "V1" && !o.IsPublic),
            "v1-public" => query.Where(o => o.ApiGroup == "V1" && o.IsPublic),
            "v2" => query.Where(o => o.ApiGroup == "V2"),
            _ => throw new NotSupportedException("Unsupported criteria.")
        };
    }
}
