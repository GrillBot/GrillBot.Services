using GrillBot.Core.RabbitMQ.V2.Messages;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using PointsService.Core.Entity;
using PointsService.Models.Events;

namespace PointsService.Handlers.Abstractions;

public abstract class BasePointsEvent<TPayload>(
    IServiceProvider serviceProvider
) : BaseEventHandlerWithDb<TPayload, PointsServiceContext>(serviceProvider) where TPayload : class, IRabbitMessage, new()
{
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

    protected Task EnqueueUsersForRecalculationAsync(IEnumerable<(string guildId, string userId)> users)
    {
        var payloads = users.Select(o => new UserRecalculationPayload(o.guildId, o.userId)).ToList();
        return Publisher.PublishAsync(payloads);
    }
}
