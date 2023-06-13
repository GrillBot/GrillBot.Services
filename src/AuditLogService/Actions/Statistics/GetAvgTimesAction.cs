using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetAvgTimesAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public GetAvgTimesAction(AuditLogServiceContext context)
    {
        Context = context;
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

        return new ApiResult(StatusCodes.Status200OK, result);
    }

    private async Task<Dictionary<string, double>> GetAvgTimesOfApiAsync(string apiGroupName)
    {
        return await Context.ApiRequests.AsNoTracking()
            .Where(o => o.ApiGroupName == apiGroupName)
            .GroupBy(o => o.LogItem.CreatedAt.Date)
            .OrderBy(o => o.Key)
            .Select(o => new { o.Key, AvgTimes = Math.Round(o.Average(x => (x.EndAt - x.StartAt).TotalMilliseconds)) })
            .ToDictionaryAsync(o => $"{o.Key:dd. MM. yyyy}", o => o.AvgTimes);
    }

    private async Task<Dictionary<string, double>> GetAvgTimesOfJobsAsync()
    {
        return await Context.JobExecutions.AsNoTracking()
            .GroupBy(o => o.LogItem.CreatedAt.Date)
            .OrderBy(o => o.Key)
            .Select(o => new { o.Key, AvgTimes = Math.Round(o.Average(x => (x.EndAt - x.StartAt).TotalMilliseconds)) })
            .ToDictionaryAsync(o => $"{o.Key:dd. MM. yyyy}", o => o.AvgTimes);
    }

    private async Task<Dictionary<string, double>> GetAvgTimesOfInteractionsAsync()
    {
        return await Context.InteractionCommands.AsNoTracking()
            .GroupBy(o => o.LogItem.CreatedAt.Date)
            .OrderBy(o => o.Key)
            .Select(o => new { o.Key, AvgTimes = Math.Round(o.Average(x => x.Duration)) })
            .ToDictionaryAsync(o => $"{o.Key:dd. MM. yyyy}", o => o.AvgTimes);
    }
}
