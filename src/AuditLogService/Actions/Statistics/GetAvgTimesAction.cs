using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetAvgTimesAction : ApiActionBase
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetAvgTimesAction(AuditLogStatisticsContext statisticsContext)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var result = new AvgExecutionTimes
        {
            Interactions = await GetAvgTimesOfInteractionsAsync(),
            Jobs = await GetAvgTimesOfJobsAsync(),
            ExternalApi = await GetAvgTimesOfApiAsync("V2"),
            InternalApi = await GetAvgTimesOfApiAsync("V1")
        };

        return ApiResult.Ok(result);
    }

    private async Task<Dictionary<string, double>> GetAvgTimesOfApiAsync(string apiGroupName)
    {
        var baseQuery = StatisticsContext.DailyAvgTimes.AsNoTracking()
            .OrderBy(o => o.Date);

        var queryData = apiGroupName == "V2" ?
            baseQuery.Select(o => new { o.Date, AvgTimes = Math.Round(o.ExternalApi) }) :
            baseQuery.Select(o => new { o.Date, AvgTimes = Math.Round(o.InternalApi) });

        return await queryData
            .Where(o => o.AvgTimes > -1)
            .ToDictionaryAsync(o => $"{o.Date:dd. MM. yyyy}", o => o.AvgTimes);
    }

    private async Task<Dictionary<string, double>> GetAvgTimesOfJobsAsync()
    {
        return await StatisticsContext.DailyAvgTimes.AsNoTracking()
            .OrderBy(o => o.Date)
            .Where(o => o.Jobs > -1)
            .Select(o => new { o.Date, AvgTimes = Math.Round(o.Jobs) })
            .ToDictionaryAsync(o => $"{o.Date:dd. MM. yyyy}", o => o.AvgTimes);
    }

    private async Task<Dictionary<string, double>> GetAvgTimesOfInteractionsAsync()
    {
        return await StatisticsContext.DailyAvgTimes.AsNoTracking()
            .OrderBy(o => o.Date)
            .Where(o => o.Interactions > -1)
            .Select(o => new { o.Date, AvgTimes = Math.Round(o.Interactions) })
            .ToDictionaryAsync(o => $"{o.Date:dd. MM. yyyy}", o => o.AvgTimes);
    }
}
