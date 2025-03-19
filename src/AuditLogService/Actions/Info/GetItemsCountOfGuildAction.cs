using AuditLogService.Core.Entity;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;

namespace AuditLogService.Actions.Info;

public class GetItemsCountOfGuildAction(
    AuditLogServiceContext context,
    ICounterManager counterManager
) : ApiAction<AuditLogServiceContext>(counterManager, context)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var query = DbContext.LogItems.Where(o => o.GuildId == guildId);
        var count = await ContextHelper.ReadCountAsync(query);

        return ApiResult.Ok(count);
    }
}
