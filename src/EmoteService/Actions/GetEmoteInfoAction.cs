using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using EmoteService.Models.Response;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Actions;

public class GetEmoteInfoAction : ApiAction<EmoteServiceContext>
{
    public GetEmoteInfoAction(ICounterManager counterManager, EmoteServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var currentGuildId = (string)Parameters[0]!;
        var emote = Emote.Parse((string)Parameters[1]!);

        var emoteInfo = new EmoteInfo
        {
            EmoteName = emote.Name,
            EmoteUrl = emote.Url,
            IsEmoteAnimated = emote.Animated,
            OwnerGuildId = await GetOwnerGuildIdAsync(emote),
            Statistics = await GetStatisticsAsync(emote, currentGuildId)
        };

        return ApiResult.Ok(emoteInfo);
    }

    private async Task<string?> GetOwnerGuildIdAsync(Emote emote)
    {
        var query = DbContext.EmoteDefinitions.WithEmoteQuery(emote).Select(o => o.GuildId);
        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
    }

    private async Task<EmoteInfoStatistics?> GetStatisticsAsync(Emote emote, string guildId)
    {
        var baseQuery = DbContext.EmoteUserStatItems.Where(o => o.GuildId == guildId).WithEmoteQuery(emote);
        if (!await ContextHelper.IsAnyAsync(baseQuery))
            return null;

        var statistics = new EmoteInfoStatistics();

        using (CreateCounter("Database"))
        {
            statistics.FirstOccurenceUtc = await baseQuery.MinAsync(o => o.FirstOccurence);
            statistics.LastOccurenceUtc = await baseQuery.MaxAsync(o => o.LastOccurence);
            statistics.UseCount = await baseQuery.SumAsync(o => o.UseCount);
            statistics.UsersCount = await baseQuery.CountAsync();
            statistics.TopUsers = await baseQuery
                .OrderByDescending(o => o.UseCount)
                .Take(10)
                .Select(o => new { o.UserId, o.UseCount })
                .ToDictionaryAsync(o => o.UserId, o => o.UseCount);
        }

        return statistics;
    }
}
