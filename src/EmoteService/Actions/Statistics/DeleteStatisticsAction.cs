using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Actions.Statistics;

public class DeleteStatisticsAction : ApiAction
{
    private readonly EmoteServiceContext _dbContext;

    public DeleteStatisticsAction(ICounterManager counterManager, EmoteServiceContext dbContext) : base(counterManager)
    {
        _dbContext = dbContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var emote = Emote.Parse((string)Parameters[1]!);
        var userId = Parameters.ElementAtOrDefault(2) as string;

        var statisticsQuery = _dbContext.EmoteUserStatItems
            .Where(o => o.GuildId == guildId)
            .WithEmoteQuery(emote);

        if (!string.IsNullOrEmpty(userId))
            statisticsQuery = statisticsQuery.Where(o => o.UserId == userId);

        List<EmoteUserStatItem> statistics;
        using (CreateCounter("Database"))
            statistics = await statisticsQuery.ToListAsync();

        if (statistics.Count == 0)
            return ApiResult.NotFound();

        _dbContext.RemoveRange(statistics);

        int deletedRows;
        using (CreateCounter("Database"))
            deletedRows = await _dbContext.SaveChangesAsync();

        return ApiResult.Ok(deletedRows);
    }
}
