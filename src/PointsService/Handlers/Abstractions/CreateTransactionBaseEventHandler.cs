using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models.Events;

namespace PointsService.Handlers.Abstractions;

public abstract class CreateTransactionBaseEventHandler<TPayload> : BaseEventWithDb<TPayload> where TPayload : CreateTransactionBasePayload
{
    protected ILogger Logger { get; }

    protected CreateTransactionBaseEventHandler(ILoggerFactory loggerFactory, PointsServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher) : base(loggerFactory, dbContext, counterManager, publisher)
    {
        Logger = loggerFactory.CreateLogger(GetType());
    }

    protected async Task<User?> FindUserAsync(TPayload payload)
    {
        using (CreateCounter("Database"))
            return await DbContext.Users.AsNoTracking().FirstOrDefaultAsync(o => o.GuildId == payload.GuildId && o.Id == payload.UserId);
    }

    protected static Transaction CreateTransaction(TPayload payload)
    {
        return new Transaction
        {
            GuildId = payload.GuildId,
            CreatedAt = DateTime.UtcNow,
            UserId = payload.UserId
        };
    }

    protected bool ValidationFailed(string message)
    {
        Logger.LogWarning("{message}", message);
        return false;
    }

    protected Task EnqueUserForRecalculationAsync(TPayload payload)
    {
        var recalcPayload = new UserRecalculationPayload
        {
            GuildId = payload.GuildId,
            UserId = payload.UserId
        };

        return Publisher.PublishAsync(UserRecalculationPayload.QueueName, recalcPayload);
    }
}
