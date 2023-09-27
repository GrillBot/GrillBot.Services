using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

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
        var method = logItem.ApiRequest!.Method;
        var templatePath = logItem.ApiRequest!.TemplatePath;
        var endpoint = $"{method} {templatePath}";
        var stats = await GetOrCreateStatisticEntity<ApiRequestStat>(o => o.Endpoint == endpoint, endpoint);

        var deletedItems = await Context.LogItems.AsNoTracking()
            .Where(o => o.Type == LogType.Api && o.IsDeleted)
            .Select(o => o.Id)
            .ToListAsync();

        var dataQuery = Context.ApiRequests.AsNoTracking()
            .Where(o => o.Method == method && o.TemplatePath == templatePath);

        if (deletedItems.Count > 0)
            dataQuery = dataQuery.Where(o => deletedItems.Contains(o.LogItemId));

        var data = await dataQuery.GroupBy(_ => 1).Select(g => new
        {
            LastRequest = g.Max(x => x.EndAt),
            FailedCount = g.LongCount(x => !x.IsSuccess),
            MaxDuration = (int)g.Max(x => x.Duration),
            MinDuration = (int)g.Min(x => x.Duration),
            SuccessCount = g.LongCount(x => x.IsSuccess),
            TotalDuration = (int)g.Sum(x => x.Duration),
            LastRunDuration = g.OrderByDescending(x => x.EndAt).Select(x => (int)x.Duration).First()
        }).FirstOrDefaultAsync();

        stats.Endpoint = endpoint;
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
}
