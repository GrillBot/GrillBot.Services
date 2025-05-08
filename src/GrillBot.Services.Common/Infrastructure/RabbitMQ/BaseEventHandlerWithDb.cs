using GrillBot.Core.RabbitMQ.V2.Messages;
using GrillBot.Services.Common.EntityFramework.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GrillBot.Services.Common.Infrastructure.RabbitMQ;

public abstract class BaseEventHandlerWithDb<TPayload, TDbContext>
    : BaseEventHandler<TPayload>
    where TPayload : class, IRabbitMessage, new()
    where TDbContext : DbContext
{
    protected TDbContext DbContext { get; }
    protected ContextHelper<TDbContext> ContextHelper { get; }

    protected BaseEventHandlerWithDb(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        DbContext = serviceProvider.GetRequiredService<TDbContext>();
        ContextHelper = new ContextHelper<TDbContext>(CounterManager, DbContext, CounterKey);
    }
}
