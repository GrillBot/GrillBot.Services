using PointsService.Core.Entity;
using PointsService.Core.Repository;

namespace PointsService.BackgroundServices.PostProcessAction;

public class RecalculateLeaderboardAction : PostProcessActionBase
{
    public RecalculateLeaderboardAction(PointsServiceRepository repository) : base(repository)
    {
    }

    public override async Task ProcessAsync()
    {
        var request = GetParameter<PostProcessRequest>();

        if (request is null)
        {
            var user = GetParameter<User>();
            if (user is not null && !await Repository.Leaderboard.HaveLeaderboardRecordAsync(user.GuildId, user.Id))
                await ProcessUserAsync(user.GuildId, user.Id);
        }
        else
        {
            await ProcessUserAsync(request.Transaction.GuildId, request.Transaction.UserId);
        }

        await Repository.CommitAsync();
    }

    private async Task ProcessUserAsync(string guildId, string userId)
    {
        var item = await Repository.Leaderboard.FindItemAsync(guildId, userId);
        var isNewItem = item is null;

        item ??= new LeaderboardItem
        {
            GuildId = guildId,
            UserId = userId
        };

        var currentStatus = await Repository.Transaction.ComputePointsStatusAsync(guildId, userId, false);
        if (currentStatus.Total == 0) return;

        item.Today = currentStatus.Today;
        item.Total = currentStatus.Total;
        item.YearBack = currentStatus.YearBack;
        item.MonthBack = currentStatus.MonthBack;

        if (isNewItem)
            await Repository.AddAsync(item);
    }
}
