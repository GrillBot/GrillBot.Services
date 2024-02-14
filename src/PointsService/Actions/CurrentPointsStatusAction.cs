using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core;
using PointsService.Core.Entity;
using PointsService.Models;

namespace PointsService.Actions;

public class CurrentPointsStatusAction : ApiAction
{
    public CurrentPointsStatusAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher)
        : base(counterManager, dbContext, publisher)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var userId = (string)Parameters[1]!;

        var leaderboardItem = await FindLeaderboardItemAsync(guildId, userId);
        var result = new PointsStatus
        {
            MonthBack = leaderboardItem.MonthBack,
            Today = leaderboardItem.Today,
            Total = leaderboardItem.Total,
            YearBack = leaderboardItem.YearBack
        };

        return ApiResult.Ok(result);
    }

    private async Task<LeaderboardItem> FindLeaderboardItemAsync(string guildId, string userId)
    {
        var query = DbContext.Leaderboard.AsNoTracking()
            .Where(o => o.GuildId == guildId && o.UserId == userId);

        using (CreateCounter("Database"))
            return (await query.FirstOrDefaultAsync()) ?? new LeaderboardItem();
    }
}
