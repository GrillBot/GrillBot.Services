using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using PointsService.Core.Entity;
using PointsService.Models.Events;

namespace PointsService.Handlers.Abstractions;

public abstract class BasePointsEvent<TPayload>(
    ILoggerFactory loggerFactory,
    PointsServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher
) : BaseEventHandlerWithDb<TPayload, PointsServiceContext>(loggerFactory, dbContext, counterManager, publisher) where TPayload : class, new()
{
    public override string TopicName => "Points";

    protected async Task<User> FindOrCreateUserAsync(string guildId, string userId)
    {
        var userQuery = DbContext.Users.Where(o => o.GuildId == guildId && o.Id == userId);
        var entity = await ContextHelper.ReadFirstOrDefaultEntityAsync(userQuery);

        if (entity is null)
        {
            entity = new User
            {
                Id = userId,
                GuildId = guildId,
                IsUser = true,
                PointsPosition = int.MaxValue
            };

            await DbContext.AddAsync(entity);
        }

        return entity;
    }

    protected Task EnqueueUserForRecalculationAsync(string guildId, string userId)
        => Publisher.PublishAsync("Points", new UserRecalculationPayload(guildId, userId), "UserRecalculation");

    protected Task EnqueueUsersForRecalculationAsync(IEnumerable<(string guildId, string userId)> users)
    {
        var payloads = users.Select(o => new UserRecalculationPayload(o.guildId, o.userId)).ToList();
        return Publisher.PublishAsync("Points", payloads, "UserRecalculation");
    }
}
