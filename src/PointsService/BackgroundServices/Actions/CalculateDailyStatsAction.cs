using PointsService.Core.Entity;

namespace PointsService.BackgroundServices.Actions;

public class CalculateDailyStatsAction : PostProcessActionBase
{
    public CalculateDailyStatsAction(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(User user)
    {
        var computedFullStats = await Repository.Transaction.ComputeAllDaysStatsAsync(user.GuildId, user.Id);
        if (computedFullStats.Count == 0)
            return;

        var currentAllStats = await Repository.DailyStats.FindAllStatsForUserAsync(user.GuildId, user.Id);
        foreach (var item in computedFullStats)
        {
            var stats = currentAllStats.Find(o => o.Date == item.day);
            if (stats is null)
            {
                stats = new DailyStat
                {
                    Date = item.day,
                    UserId = user.Id,
                    GuildId = user.GuildId
                };

                await Repository.AddAsync(stats);
            }

            stats.MessagePoints = item.messagePoints;
            stats.ReactionPoints = item.reactionPoints;
        }

        await Repository.CommitAsync();
    }
}
