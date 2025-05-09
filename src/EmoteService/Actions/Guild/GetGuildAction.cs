using EmoteService.Core.Entity;
using EmoteService.Models.Response.Guild;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Actions.Guild;

public class GetGuildAction(
    ICounterManager counterManager,
    EmoteServiceContext dbContext
) : ApiAction<EmoteServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<ulong>(0);

        var query = DbContext.Guilds.AsNoTracking()
            .Where(o => o.GuildId == guildId)
            .Select(o => new GuildData(
                o.SuggestionChannelId,
                o.VoteChannelId,
                o.VoteTime
            ));

        var guild = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
        return ApiResult.Ok(guild ?? new GuildData(null, null, TimeSpan.MinValue));
    }
}
