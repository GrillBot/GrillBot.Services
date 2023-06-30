using PointsService.Core.Entity;
using PointsService.Core.Repository;

namespace PointsService.BackgroundServices.Actions;

public class RecalculatePositionAction : PostProcessActionBase
{
    public RecalculatePositionAction(PointsServiceRepository repository) : base(repository)
    {
    }

    public override async Task ProcessAsync(User user)
    {
        // Entity in parameter is not tracked.
        var userEntity = await Repository.User.FindUserAsync(user.GuildId, user.Id);
        if (userEntity is null)
            return;

        var position = await Repository.Leaderboard.ComputePositionAsync(user.GuildId, user.Id);
        if (position == 0)
            position = await Repository.Leaderboard.ComputeMaxPositionAsync(user.GuildId);
        if (userEntity.PointsPosition != position)
            userEntity.PointsPosition = position;

        var samePositionUsers = await Repository.User.FindUsersWithSamePositionAsync(user.GuildId, user.Id, position);
        foreach (var samePositionUser in samePositionUsers)
        {
            // Check if user have some points.
            if (await Repository.Leaderboard.HaveSomePointsAsync(samePositionUser.GuildId, samePositionUser.Id))
                samePositionUser.PendingRecalculation = true;
        }

        await Repository.CommitAsync();
    }
}
