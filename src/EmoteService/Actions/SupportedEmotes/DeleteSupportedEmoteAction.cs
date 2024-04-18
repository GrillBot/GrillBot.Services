using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;

namespace EmoteService.Actions.SupportedEmotes;

public class DeleteSupportedEmoteAction : ApiAction<EmoteServiceContext>
{
    public DeleteSupportedEmoteAction(ICounterManager counterManager, EmoteServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<string>(0);
        var emoteId = GetParameter<string>(1);
        var emote = Emote.Parse(emoteId);

        var query = DbContext.EmoteDefinitions
            .Where(o => o.GuildId == guildId)
            .WithEmoteQuery(emote);
        var item = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
        if (item is null)
            return ApiResult.NotFound();

        DbContext.Remove(item);
        await DbContext.SaveChangesAsync();
        return ApiResult.Ok();
    }
}
