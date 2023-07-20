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
        var currentStats = await GetOrCreateStatisticAsync(action, apiGroup, isPublic, userId);

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

        currentStats.Count = await countQuery.CountAsync();
        await StatisticsContext.SaveChangesAsync();
    }

    private async Task<ApiUserActionStatistic> GetOrCreateStatisticAsync(string action, string apiGroup, bool isPublic, string userId)
    {
        var stats = await StatisticsContext.ApiUserActionStatistics
            .FirstOrDefaultAsync(o => o.Action == action && o.ApiGroup == apiGroup && o.IsPublic == isPublic && o.UserId == userId);

        if (stats is null)
        {
            stats = new ApiUserActionStatistic
            {
                UserId = userId,
                IsPublic = isPublic,
                Action = action,
                ApiGroup = apiGroup
            };

            await StatisticsContext.AddAsync(stats);
        }

        return stats;
    }
}
