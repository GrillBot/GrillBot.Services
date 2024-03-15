using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;

namespace EmoteService.Actions.Statistics;

public class DeleteStatisticsAction : ApiAction<EmoteServiceContext>
{
    public DeleteStatisticsAction(ICounterManager counterManager, EmoteServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var emote = Emote.Parse((string)Parameters[1]!);
        var userId = Parameters.ElementAtOrDefault(2) as string;

        var statisticsQuery = DbContext.EmoteUserStatItems
            .Where(o => o.GuildId == guildId)
            .WithEmoteQuery(emote);

        if (!string.IsNullOrEmpty(userId))
            statisticsQuery = statisticsQuery.Where(o => o.UserId == userId);

        var statistics = await ContextHelper.ReadEntitiesAsync(statisticsQuery);
        if (statistics.Count == 0)
            return ApiResult.NotFound();

        DbContext.RemoveRange(statistics);

        var deletedRows = await ContextHelper.SaveChagesAsync();
        return ApiResult.Ok(deletedRows);
    }
}
