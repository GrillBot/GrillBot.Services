using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetAvgTimesAction(
    AuditLogStatisticsContext statisticsContext,
    ICounterManager counterManager
) : ApiAction<AuditLogStatisticsContext>(counterManager, statisticsContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var result = new AvgExecutionTimes
        {
            Interactions = await GetAvgTimesOfInteractionsAsync(),
            Jobs = await GetAvgTimesOfJobsAsync(),
            ExternalApi = await GetAvgTimesOfApiAsync("V2"),
            InternalApi = await GetAvgTimesOfApiAsync("V3")
        };

        return ApiResult.Ok(result);
    }

    private async Task<Dictionary<string, double>> GetAvgTimesOfApiAsync(string apiGroupName)
    {
        var baseQuery = DbContext.DailyAvgTimes.AsNoTracking().OrderBy(o => o.Date);

        var queryData = apiGroupName == "V2" ?
            baseQuery.Select(o => new { o.Date, AvgTimes = Math.Round(o.ExternalApi) }) :
            baseQuery.Select(o => new { o.Date, AvgTimes = Math.Round(o.InternalApi) });

        queryData = queryData.Where(o => o.AvgTimes > -1);
        return await ContextHelper.ReadToDictionaryAsync(queryData, o => $"{o.Date:dd. MM. yyyy}", o => o.AvgTimes);
    }

    private async Task<Dictionary<string, double>> GetAvgTimesOfJobsAsync()
    {
        var query = DbContext.DailyAvgTimes.AsNoTracking()
            .OrderBy(o => o.Date)
            .Where(o => o.Jobs > -1)
            .Select(o => new { o.Date, AvgTimes = Math.Round(o.Jobs) });

        return await ContextHelper.ReadToDictionaryAsync(query, o => $"{o.Date:dd. MM. yyyy}", o => o.AvgTimes);
    }

    private async Task<Dictionary<string, double>> GetAvgTimesOfInteractionsAsync()
    {
        var query = DbContext.DailyAvgTimes.AsNoTracking()
            .OrderBy(o => o.Date)
            .Where(o => o.Interactions > -1)
            .Select(o => new { o.Date, AvgTimes = Math.Round(o.Interactions) });

        return await ContextHelper.ReadToDictionaryAsync(query, o => $"{o.Date:dd. MM. yyyy}", o => o.AvgTimes);
    }
}
