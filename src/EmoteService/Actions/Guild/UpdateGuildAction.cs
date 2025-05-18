using EmoteService.Core.Entity;
using EmoteService.Models.Request.Guild;
using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;

namespace EmoteService.Actions.Guild;

public class UpdateGuildAction(
    ICounterManager counterManager,
    EmoteServiceContext dbContext
) : ApiAction<EmoteServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<ulong>(0);
        var request = GetParameter<GuildRequest>(1);

        var guildQuery = DbContext.Guilds.Where(o => o.GuildId == guildId);
        var guild = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildQuery);

        if (guild == null)
        {
            guild = new Core.Entity.Guild
            {
                GuildId = guildId
            };

            await DbContext.AddAsync(guild);
        }

        guild.SuggestionChannelId = request.SuggestionChannelId?.ToUlong() ?? 0;
        guild.VoteChannelId = request.VoteChannelId?.ToUlong() ?? 0;
        guild.VoteTime = request.VoteTime;
        await ContextHelper.SaveChangesAsync();

        return ApiResult.Ok();
    }
}
