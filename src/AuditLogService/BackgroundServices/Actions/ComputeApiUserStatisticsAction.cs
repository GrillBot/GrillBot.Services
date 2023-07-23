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
        var isPublic = logItem.ApiRequest!.Identification.StartsWith("ApiV1(Public/");
        var userId = logItem.UserId ?? logItem.ApiRequest!.Identification;
        var stats = await GetOrCreateStatisticEntity<ApiUserActionStatistic>(
            o => o.Action == action && o.ApiGroup == apiGroup && o.IsPublic == isPublic && o.UserId == userId,
            action, userId, apiGroup, isPublic
        );

        var countQuery = Context.ApiRequests.AsNoTracking()
            .Where(o => !string.IsNullOrEmpty(o.LogItem.UserId) || o.Identification != "UnknownIdentification")
            .Where(o => o.Method == logItem.ApiRequest.Method && o.TemplatePath == logItem.ApiRequest.TemplatePath && (o.LogItem.UserId ?? o.Identification) == userId);
        if (apiGroup == "V2")
        {
            countQuery = countQuery.Where(o => o.ApiGroupName == "V2");
        }
        else
        {
            countQuery = countQuery.Where(o => o.ApiGroupName == "V1");

            if (isPublic)
                countQuery = countQuery.Where(o => o.Identification.StartsWith("ApiV1(Public/"));
            else
                countQuery = countQuery.Where(o => o.Identification.StartsWith("ApiV1(Private/"));
        }

        stats.UserId = userId;
        stats.IsPublic = isPublic;
        stats.Action = action;
        stats.ApiGroup = apiGroup;
        stats.Count = await countQuery.CountAsync();
        await StatisticsContext.SaveChangesAsync();
    }
}
