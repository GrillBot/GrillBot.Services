using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Info.Dashboard;
using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Dashboard;

public class GetTodayAvgTimesDashboard(
    AuditLogStatisticsContext statisticsContext,
    ICounterManager counterManager
) : ApiAction<AuditLogStatisticsContext>(counterManager, statisticsContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var data = await ReadTodayDailyStatsAsync();
        var result = new TodayAvgTimes
        {
            Interactions = (long)Math.Round(data.Interactions),
            Jobs = (long)Math.Round(data.Jobs),
            PublicApi = (long)Math.Round(data.ExternalApi),
            PrivateApi = (long)Math.Round(data.InternalApi)
        };

        return ApiResult.Ok(result);
    }

    private async Task<DailyAvgTimes> ReadTodayDailyStatsAsync()
    {
        var today = DateTime.UtcNow.ToDateOnly();
        var query = DbContext.DailyAvgTimes.Where(o => o.Date == today).AsNoTracking();
        var data = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);

        return data ?? new();
    }
}
