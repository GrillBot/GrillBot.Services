using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models.Events;

namespace PointsService.Core;

public abstract class ApiAction : ApiActionBase
{
    protected PointsServiceContext DbContext { get; }
    private ICounterManager CounterManager { get; }
    protected IRabbitMQPublisher Publisher { get; }

    protected ApiAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher)
    {
        DbContext = dbContext;
        CounterManager = counterManager;
        Publisher = publisher;
    }

    protected CounterItem CreateCounter(string operation)
    {
        var actionName = GetType().Name;
        if (actionName.EndsWith("Action"))
            actionName = actionName[..^"Action".Length];

        return CounterManager.Create($"Api.{actionName}.{operation}");
    }

    protected async Task<User?> FindUserAsync(string guildId, string userId)
    {
        using (CreateCounter("Database"))
            return await DbContext.Users.AsNoTracking().FirstOrDefaultAsync(o => o.GuildId == guildId && o.Id == userId);
    }

    protected Task EnqueueUserForRecalculationAsync(string guildId, string userId)
    {
        var recalcPayload = new UserRecalculationPayload(guildId, userId);
        return Publisher.PublishAsync(UserRecalculationPayload.QueueName, recalcPayload);
    }
}
