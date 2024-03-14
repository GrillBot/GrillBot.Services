using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Actions.SupportedEmotes;

public class GetIsEmoteSupportedAction : ApiAction
{
    private readonly EmoteServiceContext _dbContext;

    public GetIsEmoteSupportedAction(ICounterManager counterManager, EmoteServiceContext dbContext) : base(counterManager)
    {
        _dbContext = dbContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var emote = Emote.Parse((string)Parameters[1]!);

        bool isSupported;
        using (CreateCounter("Database"))
            isSupported = await _dbContext.EmoteDefinitions.Where(o => o.GuildId == guildId).WithEmoteQuery(emote).AnyAsync();

        return ApiResult.Ok(isSupported);
    }
}
