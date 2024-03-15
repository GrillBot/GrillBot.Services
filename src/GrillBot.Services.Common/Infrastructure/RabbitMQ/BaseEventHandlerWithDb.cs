using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.EntityFramework.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GrillBot.Services.Common.Infrastructure.RabbitMQ;

public abstract class BaseEventHandlerWithDb<TPayload, TDbContext>
    : BaseEventHandler<TPayload>
    where TPayload : IPayload, new()
    where TDbContext : DbContext
{
    protected TDbContext DbContext { get; }
    protected ContextHelper<TDbContext> ContextHelper { get; }

    protected BaseEventHandlerWithDb(ILoggerFactory loggerFactory, TDbContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, counterManager, publisher)
    {
        DbContext = dbContext;
        ContextHelper = new ContextHelper<TDbContext>(counterManager, dbContext, CounterKey);
    }
}
