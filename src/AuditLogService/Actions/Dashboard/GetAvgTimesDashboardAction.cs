using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Info.Dashboard;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Dashboard;

public class GetTodayAvgTimesDashboard : ApiActionBase
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetTodayAvgTimesDashboard(AuditLogStatisticsContext statisticsContext)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var data = await StatisticsContext.DailyAvgTimes.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Date == today);
        data ??= new DailyAvgTimes();

        var result = new TodayAvgTimes
        {
            Interactions = (long)Math.Round(data.Interactions),
            Jobs = (long)Math.Round(data.Jobs),
            PublicApi = (long)Math.Round(data.ExternalApi),
            PrivateApi = (long)Math.Round(data.InternalApi)
        };

        return ApiResult.FromSuccess(result);
    }
}
