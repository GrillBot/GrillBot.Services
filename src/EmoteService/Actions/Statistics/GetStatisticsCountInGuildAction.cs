using EmoteService.Core.Entity;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;

namespace EmoteService.Actions.Statistics;

public class GetStatisticsCountInGuildAction(
    ICounterManager counterManager,
    EmoteServiceContext dbContext
) : ApiAction<EmoteServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<string>(0);

        var query = DbContext.EmoteUserStatItems.Where(o => o.GuildId == guildId);
        var count = await ContextHelper.ReadCountAsync(query);

        return ApiResult.Ok(count);
    }
}
