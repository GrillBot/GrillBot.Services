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
        var now = GetParameter<DateTime>().ToUniversalTime();
        var yearBack = now.AddYears(-1);

        if (request is null)
        {
            var user = GetParameter<User>();
            if (user is not null && user.PointsPosition == 0)
                await ProcessUserAsync(user, now, yearBack);
        }
        else
        {
            var user = await Repository.User.FindUserAsync(request.Transaction.GuildId, request.Transaction.UserId);
            if (user is not null)
                await ProcessUserAsync(user, now, yearBack);
        }

        await Repository.CommitAsync();
    }

    private async Task ProcessUserAsync(User user, DateTime now, DateTime yearBack)
    {
        var currentStatus = await Repository.Transaction.ComputePointsStatusAsync(user.GuildId, user.Id, false, yearBack, now);
        user.PointsPosition = await Repository.Transaction.ComputePositionAsync(user.GuildId, currentStatus, yearBack);
    }
}
