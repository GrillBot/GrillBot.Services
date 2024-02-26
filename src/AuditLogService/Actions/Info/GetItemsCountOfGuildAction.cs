using AuditLogService.Core.Entity;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Info;

public class GetItemsCountOfGuildAction : ApiAction
{
    private AuditLogServiceContext DbContext { get; }

    public GetItemsCountOfGuildAction(AuditLogServiceContext context, ICounterManager counterManager) : base(counterManager)
    {
        DbContext = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;

        using (CreateCounter("Database"))
        {
            var count = await DbContext.LogItems.AsNoTracking().CountAsync(o => o.GuildId == guildId);
            return ApiResult.Ok(count);
        }
    }
}
