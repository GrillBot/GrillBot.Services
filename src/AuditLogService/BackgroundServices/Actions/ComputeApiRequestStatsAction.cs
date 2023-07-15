using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeApiRequestStatsAction : PostProcessActionBase
{
    public ComputeApiRequestStatsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Type is LogType.Api;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var successStatusCodes = Enum.GetValues<HttpStatusCode>()
            .Where(o => o < HttpStatusCode.BadRequest)
            .Select(o => $"{(int)o} ({o})")
            .ToList();

        var method = logItem.ApiRequest!.Method;
        var templatePath = logItem.ApiRequest!.TemplatePath;
        var stats = await GetOrCreateStatAsync(method, templatePath);
        var data = await Context.ApiRequests.AsNoTracking()
            .Where(o => o.Method == method && o.TemplatePath == templatePath)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                LastRequest = g.Max(x => x.EndAt),
                FailedCount = g.LongCount(x => !successStatusCodes.Contains(x.Result)),
                MaxDuration = g.Max(x => (int)Math.Round((x.EndAt - x.StartAt).TotalMilliseconds)),
                MinDuration = g.Min(x => (int)Math.Round((x.EndAt - x.StartAt).TotalMilliseconds)),
                SuccessCount = g.LongCount(x => successStatusCodes.Contains(x.Result)),
                TotalDuration = g.Sum(x => (int)Math.Round((x.EndAt - x.StartAt).TotalMilliseconds)),
                LastRunDuration = g.OrderByDescending(x => x.EndAt).Select(x => (int)Math.Round((x.EndAt - x.StartAt).TotalMilliseconds)).First()
            }).FirstOrDefaultAsync();

        if (data is null)
        {
            stats.FailedCount = 0;
            stats.SuccessCount = 0;
        }
        else
        {
            stats.LastRequest = data.LastRequest;
            stats.FailedCount = data.FailedCount;
            stats.MaxDuration = data.MaxDuration;
            stats.MinDuration = data.MinDuration;
            stats.SuccessCount = data.SuccessCount;
            stats.TotalDuration = data.TotalDuration;
            stats.LastRunDuration = data.LastRunDuration;
        }

        await StatisticsContext.SaveChangesAsync();
    }

    private async Task<ApiRequestStat> GetOrCreateStatAsync(string method, string templatePath)
    {
        var endpoint = $"{method} {templatePath}";
        var item = await StatisticsContext.RequestStats
            .FirstOrDefaultAsync(o => o.Endpoint == endpoint);

        if (item is null)
        {
            item = new ApiRequestStat { Endpoint = endpoint };
            await StatisticsContext.AddAsync(item);
        }

        return item;
    }
}
