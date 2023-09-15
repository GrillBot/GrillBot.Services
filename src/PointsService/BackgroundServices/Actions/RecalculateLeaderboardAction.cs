using PointsService.Core.Entity;
using PointsService.Core.Repository;

namespace PointsService.BackgroundServices.Actions;

public class RecalculateLeaderboardAction : PostProcessActionBase
{
    public RecalculateLeaderboardAction(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(User user)
    {
        var leaderboardItem = await Repository.Leaderboard.FindItemAsync(user.GuildId, user.Id);
        var isNewItem = leaderboardItem is null;

        leaderboardItem ??= new LeaderboardItem
        {
            GuildId = user.GuildId,
            UserId = user.Id
        };

        var currentStatus = await Repository.Transaction.ComputePointsStatusAsync(user.GuildId, user.Id, false);
        if (currentStatus.Total == 0 && isNewItem)
            return;

        leaderboardItem.Total = currentStatus.Total;
        leaderboardItem.Today = currentStatus.Today;
        leaderboardItem.YearBack = currentStatus.YearBack;
        leaderboardItem.MonthBack = currentStatus.MonthBack;

        if (isNewItem)
            await Repository.AddAsync(leaderboardItem);
        await Repository.CommitAsync();
    }
}
