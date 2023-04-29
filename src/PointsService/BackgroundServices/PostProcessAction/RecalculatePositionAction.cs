using PointsService.Core.Entity;
using PointsService.Core.Repository;

namespace PointsService.BackgroundServices.PostProcessAction;

public class RecalculatePositionAction : PostProcessActionBase
{
    public RecalculatePositionAction(PointsServiceRepository repository) : base(repository)
    {
    }

    public override async Task ProcessAsync()
    {
        var request = GetParameter<PostProcessRequest>();

        if (request is null)
        {
            var users = GetParameter<List<User>>();
            if (users is not null)
            {
                await ProcessAllAsync(users);
                await Repository.CommitAsync();
            }
        }
        else
        {
            var user = await Repository.User.FindUserAsync(request.Transaction.GuildId, request.Transaction.UserId);
            if (user is not null)
            {
                await ProcessUserAsync(user);
                await Repository.CommitAsync();
            }
        }
    }

    private async Task ProcessUserAsync(User user)
    {
        user.PointsPosition = await Repository.Leaderboard.ComputePositionAsync(user.GuildId, user.Id);
    }

    private async Task ProcessAllAsync(IEnumerable<User> users)
    {
        foreach (var perGuild in users.GroupBy(o => o.GuildId))
        {
            var leaderboard = await Repository.Leaderboard.ReadLeaderboardAsync(perGuild.Key, 0, 0, true);

            foreach (var user in perGuild)
            {
                var position = leaderboard.FindIndex(o => o.UserId == user.Id);
                if (position == -1)
                    user.PointsPosition = leaderboard.Count;
                else
                    user.PointsPosition = position + 1;
            }
        }
    }
}
