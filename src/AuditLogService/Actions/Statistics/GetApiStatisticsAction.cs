using System.Net;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetApiStatisticsAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public GetApiStatisticsAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var statistics = new ApiStatistics
        {
            ByDateInternalApi = await GetApiStatisticsByDateForApiGroupAsync("V1"),
            ByDatePublicApi = await GetApiStatisticsByDateForApiGroupAsync("V2"),
            ByStatusCodeInternalApi = await GetApiStatisticsByResultForApiGroupAsync("V1"),
            ByStatusCodePublicApi = await GetApiStatisticsByResultForApiGroupAsync("V2"),
            Endpoints = await GetEndpointStatisticsAsync()
        };

        return new ApiResult(StatusCodes.Status200OK, statistics);
    }

    private async Task<Dictionary<string, int>> GetApiStatisticsByDateForApiGroupAsync(string apiGroupName)
    {
        return await Context.ApiRequests.AsNoTracking()
            .Where(o => o.ApiGroupName == apiGroupName)
            .GroupBy(o => new { o.EndAt.Year, o.EndAt.Month })
            .OrderBy(o => o.Key.Year).ThenBy(o => o.Key.Month)
            .Select(o => new { Key = $"{o.Key.Year}-{o.Key.Month.ToString().PadLeft(2, '0')}", Count = o.Count() })
            .ToDictionaryAsync(o => o.Key, o => o.Count);
    }

    private async Task<Dictionary<string, int>> GetApiStatisticsByResultForApiGroupAsync(string apiGroupName)
    {
        return await Context.ApiRequests.AsNoTracking()
            .Where(o => o.ApiGroupName == apiGroupName)
            .GroupBy(o => o.Result)
            .OrderBy(o => o.Key)
            .Select(o => new { o.Key, Count = o.Count() })
            .ToDictionaryAsync(o => o.Key, o => o.Count);
    }

    private async Task<List<StatisticItem>> GetEndpointStatisticsAsync()
    {
        var successStatusCodes = Enum.GetValues<HttpStatusCode>()
            .Where(o => o < HttpStatusCode.BadRequest)
            .Select(o => $"{(int)o} ({o})")
            .ToList();

        var items = await Context.ApiRequests.AsNoTracking()
            .GroupBy(o => new { o.Method, o.TemplatePath })
            .Select(o => new StatisticItem
            {
                Key = $"{o.Key.Method} {o.Key.TemplatePath}",
                Last = o.Max(x => x.LogItem.CreatedAt),
                FailedCount = o.Count(x => !successStatusCodes.Contains(x.Result)),
                MaxDuration = o.Max(x => (int)Math.Round((x.EndAt - x.StartAt).TotalMilliseconds)),
                MinDuration = o.Min(x => (int)Math.Round((x.EndAt - x.StartAt).TotalMilliseconds)),
                SuccessCount = o.Count(x => successStatusCodes.Contains(x.Result)),
                TotalDuration = o.Sum(x => (int)Math.Round((x.EndAt - x.StartAt).TotalMilliseconds))
            })
            .ToListAsync();

        foreach (var item in items)
        {
            var fields = item.Key.Split(' ');
            var lastItem = await Context.ApiRequests.AsNoTracking()
                .Where(o => o.Method == fields[0] && o.TemplatePath == fields[1])
                .OrderByDescending(x => x.LogItem.CreatedAt)
                .FirstOrDefaultAsync();
            if (lastItem is null)
                continue;
            item.LastRunDuration = (int)Math.Round((lastItem.EndAt - lastItem.StartAt).TotalMilliseconds);
        }

        return items;
    }
}
