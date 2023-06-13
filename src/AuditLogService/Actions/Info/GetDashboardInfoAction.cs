using System.Net;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;
using GrillBot.Core.Helpers;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Info;

public class GetDashboardInfoAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public GetDashboardInfoAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var result = new DashboardInfo
        {
            InternalApi = await GetApiDashboardInfoAsync("V1"),
            PublicApi = await GetApiDashboardInfoAsync("V2"),
            Interactions = await GetInteractionDashboardInfoAsync(),
            Jobs = await GetJobDashboardInfoAsync(),
            TodayAvgTimes = await ComputeTodayAvgTimesAsync()
        };

        return new ApiResult(StatusCodes.Status200OK, result);
    }

    private async Task<List<DashboardInfoRow>> GetApiDashboardInfoAsync(string apiGroupName)
    {
        return await Context.ApiRequests.AsNoTracking()
            .Where(o => o.ApiGroupName == apiGroupName && o.ActionName != "GetDashboardAsync")
            .OrderByDescending(o => o.EndAt)
            .Select(o => new DashboardInfoRow
            {
                Duration = (int)Math.Round((o.EndAt - o.StartAt).TotalMilliseconds),
                Name = $"{o.Method} {o.TemplatePath}",
                Result = o.Result
            })
            .Take(10)
            .ToListAsync();
    }

    private async Task<List<DashboardInfoRow>> GetInteractionDashboardInfoAsync()
    {
        return await Context.InteractionCommands.AsNoTracking()
            .OrderByDescending(o => o.LogItem.CreatedAt)
            .Select(o => new DashboardInfoRow
            {
                Duration = o.Duration,
                Name = $"{o.Name} ({o.ModuleName}/{o.MethodName})",
                Success = o.IsSuccess
            })
            .Take(10)
            .ToListAsync();
    }

    private async Task<List<DashboardInfoRow>> GetJobDashboardInfoAsync()
    {
        return await Context.JobExecutions.AsNoTracking()
            .OrderByDescending(o => o.EndAt)
            .Select(o => new DashboardInfoRow
            {
                Duration = (int)Math.Round((o.EndAt - o.StartAt).TotalMilliseconds),
                Name = o.JobName,
                Success = !o.WasError
            })
            .Take(10)
            .ToListAsync();
    }

    private async Task<TodayAvgTimes> ComputeTodayAvgTimesAsync()
    {
        var startOfDay = DateTime.UtcNow.Date;
        var endOfDay = DateHelper.EndOfDayUtc;

        var interactions = await Context.InteractionCommands.AsNoTracking()
            .Where(o => o.LogItem.CreatedAt >= startOfDay && o.LogItem.CreatedAt < endOfDay)
            .AverageAsync(o => (long?)o.Duration) ?? 0;

        var jobs = await Context.JobExecutions.AsNoTracking()
            .Where(o => o.LogItem.CreatedAt >= startOfDay && o.LogItem.CreatedAt < endOfDay)
            .Select(o => (long)Math.Round((o.EndAt - o.StartAt).TotalMilliseconds))
            .AverageAsync(o => (long?)o) ?? 0;

        var apiBaseQuery = Context.ApiRequests.AsNoTracking()
            .Where(o => o.LogItem.CreatedAt >= startOfDay && o.LogItem.CreatedAt < endOfDay)
            .Select(o => new { Duration = (long)Math.Round((o.EndAt - o.StartAt).TotalMilliseconds), o.ApiGroupName });

        var publicApi = await apiBaseQuery.Where(o => o.ApiGroupName == "V2").AverageAsync(o => (long?)o.Duration) ?? 0;
        var privateApi = await apiBaseQuery.Where(o => o.ApiGroupName == "V1").AverageAsync(o => (long?)o.Duration) ?? 0;

        return new TodayAvgTimes
        {
            Interactions = (long)Math.Round(interactions),
            Jobs = (long)Math.Round(jobs),
            PublicApi = (long)Math.Round(publicApi),
            PrivateApi = (long)Math.Round(privateApi)
        };
    }
}
