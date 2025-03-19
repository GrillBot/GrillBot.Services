using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Core.Extensions;
using AuditLogService.Managers;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Internal;

public class TriggerRecalculationAction(
    ICounterManager counterManager,
    AuditLogServiceContext dbContext,
    DataRecalculationManager _recalculationManager
) : ApiAction<AuditLogServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var type = GetParameter<LogType>(0);
        var logItems = await ReadLogItemsAsync(type);

        await _recalculationManager.EnqueueRecalculationAsync(logItems);
        return ApiResult.Ok();
    }

    private async Task<List<LogItem>> ReadLogItemsAsync(LogType type)
    {
        var query = DbContext.LogItems.AsNoTracking().Where(o => o.Type == type && !o.IsDeleted);
        var items = await ContextHelper.ReadEntitiesAsync(query);

        foreach (var chunk in items.Chunk(1000))
        {
            switch (type)
            {
                case LogType.Api:
                    await chunk.SetLogDataAsync(DbContext.ApiRequests, (logItem, data) => logItem.ApiRequest = data, ContextHelper, true);
                    break;
                case LogType.InteractionCommand:
                    await chunk.SetLogDataAsync(DbContext.InteractionCommands, (logItem, data) => logItem.InteractionCommand = data, ContextHelper, true);
                    break;
                case LogType.JobCompleted:
                    await chunk.SetLogDataAsync(DbContext.JobExecutions, (logItem, data) => logItem.Job = data, ContextHelper, true);
                    break;
            }
        }

        return items;
    }
}
