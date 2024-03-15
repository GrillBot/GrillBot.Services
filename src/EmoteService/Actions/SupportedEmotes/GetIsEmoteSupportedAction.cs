using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;

namespace EmoteService.Actions.SupportedEmotes;

public class GetIsEmoteSupportedAction : ApiAction<EmoteServiceContext>
{
    public GetIsEmoteSupportedAction(ICounterManager counterManager, EmoteServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var emote = Emote.Parse((string)Parameters[1]!);

        var query = DbContext.EmoteDefinitions.Where(o => o.GuildId == guildId).WithEmoteQuery(emote);
        var isSupported = await ContextHelper.IsAnyAsync(query);
        return ApiResult.Ok(isSupported);
    }
}
