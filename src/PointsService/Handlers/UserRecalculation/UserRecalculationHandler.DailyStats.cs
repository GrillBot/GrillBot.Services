using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Handlers.UserRecalculation;

public partial class UserRecalculationHandler
{
    private async Task ComputeDailyStatsAsync(User user)
    {
        var fullStatsQuery = DbContext.Transactions.AsNoTracking()
            .Where(o => o.GuildId == user.GuildId && o.UserId == user.Id)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(o => new DailyStat
            {
                Date = new DateOnly(o.Key.Year, o.Key.Month, o.Key.Day),
                MessagePoints = o.Where(x => x.ReactionId == "").Sum(x => x.Value),
                ReactionPoints = o.Where(x => x.ReactionId != "").Sum(x => x.Value),
                GuildId = user.GuildId,
                UserId = user.Id
            });

        var currentDailyStatsQuery = DbContext.DailyStats
            .Where(o => o.GuildId == user.GuildId && o.UserId == user.Id);

        List<DailyStat> allStats;
        Dictionary<DateOnly, DailyStat> currentStats;

        using (CreateCounter("Database"))
        {
            allStats = await fullStatsQuery.ToListAsync();

            var currentStatsData = await currentDailyStatsQuery.ToListAsync();
            currentStats = currentStatsData.ToDictionary(o => o.Date, o => o);
        }

        foreach (var stat in allStats)
        {
            if (!currentStats.TryGetValue(stat.Date, out var dailyStat))
            {
                await DbContext.AddAsync(stat);
            }
            else
            {
                dailyStat.MessagePoints = stat.MessagePoints;
                dailyStat.ReactionPoints = stat.ReactionPoints;
            }
        }
    }
}
