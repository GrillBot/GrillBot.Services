using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GrillBot.Services.Common.Handlers.Abstractions;

public abstract class BaseEventHandlerWithDb<TPayload, TDbContext>
    : BaseEventHandler<TPayload>
    where TPayload : IPayload, new()
    where TDbContext : DbContext
{
    protected TDbContext DbContext { get; }

    protected BaseEventHandlerWithDb(ILoggerFactory loggerFactory, TDbContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, counterManager, publisher)
    {
        DbContext = dbContext;
    }
}
