using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;
using GrillBot.Core.Helpers;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Dashboard;

public class GetTodayAvgTimesDashboard : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public GetTodayAvgTimesDashboard(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var startOfDay = DateTime.UtcNow.Date;
        var endOfDay = DateHelper.EndOfDayUtc;

        var interactions = await Context.InteractionCommands.AsNoTracking()
            .Where(o => o.LogItem.CreatedAt >= startOfDay && o.LogItem.CreatedAt < endOfDay)
            .AverageAsync(o => (long?)o.Duration) ?? 0;

        var jobs = await Context.JobExecutions.AsNoTracking()
            .Where(o => o.EndAt >= startOfDay && o.EndAt < endOfDay)
            .Select(o => (long)Math.Round((o.EndAt - o.StartAt).TotalMilliseconds))
            .AverageAsync(o => (long?)o) ?? 0;

        var apiBaseQuery = Context.ApiRequests.AsNoTracking()
            .Where(o => o.EndAt >= startOfDay && o.EndAt < endOfDay)
            .Select(o => new { Duration = (long)Math.Round((o.EndAt - o.StartAt).TotalMilliseconds), o.ApiGroupName });

        var publicApi = await apiBaseQuery.Where(o => o.ApiGroupName == "V2").AverageAsync(o => (long?)o.Duration) ?? 0;
        var privateApi = await apiBaseQuery.Where(o => o.ApiGroupName == "V1").AverageAsync(o => (long?)o.Duration) ?? 0;

        var result = new TodayAvgTimes
        {
            Interactions = (long)Math.Round(interactions),
            Jobs = (long)Math.Round(jobs),
            PublicApi = (long)Math.Round(publicApi),
            PrivateApi = (long)Math.Round(privateApi)
        };

        return ApiResult.FromSuccess(result);
    }
}
