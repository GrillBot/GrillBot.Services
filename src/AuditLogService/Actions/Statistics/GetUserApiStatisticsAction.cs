using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetUserApiStatisticsAction : ApiAction
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetUserApiStatisticsAction(AuditLogStatisticsContext statisticsContext, ICounterManager counterManager) : base(counterManager)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var criteria = (string)Parameters[0]!;

        var query = Filter(StatisticsContext.ApiUserActionStatistics, criteria)
            .Select(o => Tuple.Create(o.Action, o.Count, o.UserId))
            .AsNoTracking();

        List<Tuple<string, long, string>> data;
        using (CreateCounter("Database"))
            data = await query.ToListAsync();

        var result = data.ConvertAll(o => new UserActionCountItem
        {
            Action = o.Item1,
            Count = o.Item2,
            UserId = o.Item3
        });

        return ApiResult.Ok(result);
    }

    private static IQueryable<ApiUserActionStatistic> Filter(IQueryable<ApiUserActionStatistic> query, string criteria)
    {
        return criteria switch
        {
            "v1-private" => query.Where(o => o.ApiGroup == "V1" && !o.IsPublic),
            "v1-public" => query.Where(o => o.ApiGroup == "V1" && o.IsPublic),
            "v2" => query.Where(o => o.ApiGroup == "V2"),
            "v3" => query.Where(o => o.ApiGroup == "V3"),
            _ => throw new NotSupportedException("Unsupported criteria.")
        };
    }
}
