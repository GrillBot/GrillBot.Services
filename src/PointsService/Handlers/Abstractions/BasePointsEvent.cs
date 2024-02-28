using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models.Events;

namespace PointsService.Handlers.Abstractions;

public abstract class BasePointsEvent<TPayload> : BaseEventHandlerWithDb<TPayload, PointsServiceContext> where TPayload : IPayload, new()
{
    protected BasePointsEvent(ILoggerFactory loggerFactory, PointsServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher) : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected async Task<User?> FindUserAsync(string guildId, string userId)
    {
        using (CreateCounter("Database"))
            return await DbContext.Users.FirstOrDefaultAsync(o => o.GuildId == guildId && o.Id == userId);
    }

    protected Task EnqueueUserForRecalculationAsync(string guildId, string userId)
        => Publisher.PublishAsync(new UserRecalculationPayload(guildId, userId));
}
