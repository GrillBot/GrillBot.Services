using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using PointsService.Core.Entity;
using PointsService.Models.Events;

namespace PointsService.Handlers.Abstractions;

public abstract class BasePointsEvent<TPayload> : BaseEventHandlerWithDb<TPayload, PointsServiceContext> where TPayload : IPayload, new()
{
    protected BasePointsEvent(ILoggerFactory loggerFactory, PointsServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher) : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

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
        => Publisher.PublishAsync(new UserRecalculationPayload(guildId, userId));
}
