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
            dataQuery = dataQuery.Where(o => !deletedItems.Contains(o.LogItemId));

        if (await dataQuery.AnyAsync())
        {
            stats.LastRequest = await dataQuery.MaxAsync(o => o.EndAt);
            stats.FailedCount = await dataQuery.CountAsync(o => !o.IsSuccess);
            stats.MaxDuration = await dataQuery.MaxAsync(o => o.Duration);
            stats.MinDuration = await dataQuery.MinAsync(o => o.Duration);
            stats.SuccessCount = await dataQuery.CountAsync(o => o.IsSuccess);
            stats.TotalDuration = await dataQuery.SumAsync(o => o.Duration);

            stats.LastRunDuration = await dataQuery
                .OrderByDescending(o => o.EndAt)
                .Select(o => o.Duration)
                .FirstOrDefaultAsync();
        }
        else
        {
            stats.FailedCount = 0;
            stats.SuccessCount = 0;
        }

        await StatisticsContext.SaveChangesAsync();
    }
}
