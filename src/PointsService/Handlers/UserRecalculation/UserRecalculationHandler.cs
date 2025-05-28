using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using PointsService.Core.Entity;
using PointsService.Handlers.Abstractions;
using PointsService.Models.Events;

namespace PointsService.Handlers.UserRecalculation;

public partial class UserRecalculationHandler(
    IServiceProvider serviceProvider
) : BasePointsEvent<UserRecalculationPayload>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(UserRecalculationPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var user = await FindOrCreateUserAsync(message.GuildId, message.UserId);

        await ProcessActionAsync(ComputeUserInfoAsync, user, nameof(ComputeUserInfoAsync));
        await ProcessActionAsync(ComputeDailyStatsAsync, user, nameof(ComputeDailyStatsAsync));
        await ProcessActionAsync(ComputeLeaderboardAsync, user, nameof(ComputeLeaderboardAsync));
        await ProcessActionAsync(ComputePositionAsync, user, nameof(ComputePositionAsync));
        await ProcessActionAsync(ComputeTelemetryAsync, user, nameof(ComputeTelemetryAsync));

        return RabbitConsumptionResult.Success;
    }

    private async Task ProcessActionAsync(Func<User, Task> action, User user, string actionName)
    {
        actionName = actionName.Replace("Async", "");

        using (CreateCounter(actionName))
            await action(user);

        await ContextHelper.SaveChangesAsync();
    }
}
