using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeApiUserStatisticsAction : PostProcessActionBase
{
    public ComputeApiUserStatisticsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Type is LogType.Api;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var action = $"{logItem.ApiRequest!.Method} {logItem.ApiRequest!.TemplatePath}";
        var apiGroup = logItem.ApiRequest.ApiGroupName;
        var isPublic = apiGroup == "V1" && logItem.ApiRequest!.Role == "User";
        var userId = logItem.UserId ?? logItem.ApiRequest!.Identification;
        var stats = await GetOrCreateStatisticEntity<ApiUserActionStatistic>(
            o => o.Action == action && o.ApiGroup == apiGroup && o.IsPublic == isPublic && o.UserId == userId,
            action, userId, apiGroup, isPublic
        );

        var deletedItems = await Context.LogItems.AsNoTracking()
            .Where(o => o.Type == LogType.Api && o.IsDeleted)
            .Select(o => o.Id)
            .ToListAsync();

        var countQuery = Context.ApiRequests.AsNoTracking()
            .Where(o => o.Method == logItem.ApiRequest.Method && o.TemplatePath == logItem.ApiRequest.TemplatePath);

        if (deletedItems.Count > 0)
            countQuery = countQuery.Where(o => !deletedItems.Contains(o.LogItemId));

        if (apiGroup == "V2")
        {
            countQuery = countQuery.Where(o => o.ApiGroupName == "V2" && o.Identification == userId);
        }
        else
        {
            countQuery = countQuery.Where(o => o.ApiGroupName == "V1" && (o.LogItem.UserId ?? o.Identification) == userId);

            if (isPublic)
                countQuery = countQuery.Where(o => o.Role == "User");
            else
                countQuery = countQuery.Where(o => o.Role == "Admin");
        }

        stats.Count = await countQuery.CountAsync();
        await StatisticsContext.SaveChangesAsync();
    }
}
