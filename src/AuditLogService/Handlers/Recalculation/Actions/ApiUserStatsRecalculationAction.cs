using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Events.Recalculation;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions;

public class ApiUserStatsRecalculationAction : RecalculationActionBase
{
    public ApiUserStatsRecalculationAction(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        var action = $"{payload.Api!.Method} {payload.Api.TemplatePath}";
        var apiGroup = payload.Api.ApiGroupName;
        var isPublic = apiGroup == "V1" && payload.Api.Identification.Contains("Public", StringComparison.InvariantCultureIgnoreCase); // Legacy. Remove after finished implementation of V3 API and new Web.
        var identification = payload.Api.Identification;

        var stats = await GetOrCreateStatEntity<ApiUserActionStatistic>(
            o => o.Action == action && o.ApiGroup == apiGroup && o.IsPublic == isPublic && o.UserId == identification,
            action, identification, apiGroup, isPublic
        );

        var dataQuery = DbContext.ApiRequests.AsNoTracking()
            .Where(o => o.Method == payload.Api.Method && o.TemplatePath == payload.Api.TemplatePath);

        if (apiGroup == "V2")
        {
            stats.Count = await dataQuery.CountAsync(o => o.ApiGroupName == "V2" && o.Identification == identification);
        }
        else
        {
            var query = dataQuery.Where(o => o.ApiGroupName == "V1" && (o.LogItem.UserId ?? o.Identification) == identification);

            if (isPublic)
                stats.Count = await query.LongCountAsync(o => o.Role == "User");
            else
                stats.Count = await query.LongCountAsync(o => o.Role == "Admin");
        }

        await StatisticsContext.SaveChangesAsync();
    }
}
