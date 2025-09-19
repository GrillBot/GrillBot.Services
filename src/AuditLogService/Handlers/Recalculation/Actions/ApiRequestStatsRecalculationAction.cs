using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Events.Recalculation;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions;

public class ApiRequestStatsRecalculationAction(IServiceProvider serviceProvider) : RecalculationActionBase(serviceProvider)
{
    public override bool CheckPreconditions(RecalculationPayload payload)
        => payload.Api is not null && !string.IsNullOrEmpty(payload.Api.ApiGroupName);

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        var method = payload.Api!.Method;
        var templatePath = payload.Api!.TemplatePath;
        var endpoint = $"({payload.Api!.ApiGroupName}) {method} {templatePath}";
        var stats = await GetOrCreateStatEntity<ApiRequestStat>(o => o.Endpoint == endpoint, endpoint);

        var dataQuery = DbContext.ApiRequests.AsNoTracking()
            .Where(o => o.Method == method && o.TemplatePath == templatePath);

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
