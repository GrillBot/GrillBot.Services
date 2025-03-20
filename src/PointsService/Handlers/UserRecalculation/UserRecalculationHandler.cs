using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using PointsService.Core.Entity;
using PointsService.Handlers.Abstractions;
using PointsService.Models.Events;

namespace PointsService.Handlers.UserRecalculation;

public partial class UserRecalculationHandler(
    ILoggerFactory loggerFactory,
    PointsServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher
) : BasePointsEvent<UserRecalculationPayload>(loggerFactory, dbContext, counterManager, publisher)
{
    public override string QueueName => "UserRecalculation";

    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(UserRecalculationPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var user = await FindOrCreateUserAsync(message.GuildId, message.UserId);

        await ProcessActionAsync(ComputeUserInfoAsync, user, nameof(ComputeUserInfoAsync));
        await ProcessActionAsync(ComputeDailyStatsAsync, user, nameof(ComputeDailyStatsAsync));
        await ProcessActionAsync(ComputeLeaderboardAsync, user, nameof(ComputeLeaderboardAsync));
        await ProcessActionAsync(ComputePositionAsync, user, nameof(ComputePositionAsync));

        return RabbitConsumptionResult.Success;
    }

    private async Task ProcessActionAsync(Func<User, Task> action, User user, string actionName)
    {
        actionName = actionName.Replace("Async", "");

        using (CreateCounter(actionName))
            await action(user);

        await ContextHelper.SaveChagesAsync();
    }
}
