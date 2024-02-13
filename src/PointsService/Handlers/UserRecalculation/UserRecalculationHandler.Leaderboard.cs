using GrillBot.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models;

namespace PointsService.Handlers.UserRecalculation;

public partial class UserRecalculationHandler
{
    private async Task ComputeLeaderboardAsync(User user)
    {
        var leaderboardItemQuery = DbContext.Leaderboard
            .Where(o => o.GuildId == user.GuildId && o.UserId == user.Id);

        LeaderboardItem? leaderboardItem;
        using (CreateCounter("ComputeLeaderboard.Database"))
            leaderboardItem = await leaderboardItemQuery.FirstOrDefaultAsync();

        var currentStatus = await ComputePointsStatusAsync(user);
        if (currentStatus.Total == 0 && leaderboardItem is null)
            return;

        if (leaderboardItem is null)
        {
            leaderboardItem = new LeaderboardItem
            {
                GuildId = user.GuildId,
                UserId = user.Id
            };

            await DbContext.AddAsync(leaderboardItem);
        }

        leaderboardItem.Total = currentStatus.Total;
        leaderboardItem.Today = currentStatus.Today;
        leaderboardItem.YearBack = currentStatus.YearBack;
        leaderboardItem.MonthBack = currentStatus.MonthBack;
    }

    private async Task<PointsStatus> ComputePointsStatusAsync(User user)
    {
        var now = DateTime.UtcNow;
        var endOfDay = DateHelper.EndOfDayUtc;
        var query = DbContext.Transactions.AsNoTracking()
            .Where(o => o.GuildId == user.GuildId && o.UserId == user.Id)
            .GroupBy(_ => 1);

        var dtoQuery = query.Select(transactions => new PointsStatus
        {
            MonthBack = transactions.Where(t => t.MergedCount == 0 && t.CreatedAt >= now.AddMonths(-1)).Sum(t => t.Value),
            Today = transactions.Where(t => t.MergedCount == 0 && t.CreatedAt >= now.Date && t.CreatedAt <= endOfDay).Sum(t => t.Value),
            Total = transactions.Sum(t => t.Value),
            YearBack = transactions.Where(t => t.MergedCount == 0 && t.CreatedAt >= now.AddYears(-1)).Sum(t => t.Value)
        });

        using (CreateCounter("ComputeLeaderboard.Database"))
            return await dtoQuery.FirstOrDefaultAsync() ?? new PointsStatus();
    }
}
