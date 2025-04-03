using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using InviteService.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace InviteService.Actions;

public class GetInvitesCountOfGuildAction(
    ICounterManager counterManager,
    InviteContext dbContext
) : ApiAction<InviteContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<ulong>(0);

        var query = DbContext.Invites.AsNoTracking()
            .Where(o => o.GuildId == guildId.ToString());

        var result = await ContextHelper.ReadCountAsync(query);
        return ApiResult.Ok(result);
    }
}
