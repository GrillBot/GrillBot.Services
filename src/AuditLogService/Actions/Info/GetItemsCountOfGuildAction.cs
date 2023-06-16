using AuditLogService.Core.Entity;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Info;

public class GetItemsCountOfGuildAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public GetItemsCountOfGuildAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var count = await Context.LogItems.AsNoTracking().CountAsync(o => o.GuildId == guildId);

        return new ApiResult(StatusCodes.Status200OK, count);
    }
}
