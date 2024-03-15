using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.EntityFramework.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GrillBot.Services.Common.Infrastructure.Api;

public abstract class ApiAction<TDbContext> : ApiAction where TDbContext : DbContext
{
    protected TDbContext DbContext { get; }
    protected ContextHelper<TDbContext> ContextHelper { get; }

    protected ApiAction(ICounterManager counterManager, TDbContext dbContext) : base(counterManager)
    {
        DbContext = dbContext;
        ContextHelper = new ContextHelper<TDbContext>(counterManager, dbContext, CounterKey);
    }
}

public abstract class ApiAction : ApiActionBase
{
    private ICounterManager CounterManager { get; }

    protected string CounterKey { get; }

    protected ApiAction(ICounterManager counterManager)
    {
        CounterManager = counterManager;
        CounterKey = CreateCounterKey();
    }

    private string CreateCounterKey()
    {
        var actionName = GetType().Name;
        if (actionName.EndsWith("Action"))
            actionName = actionName[..^"Action".Length];

        return $"Api.{actionName}";
    }

    public CounterItem CreateCounter(string operation)
        => CounterManager.Create($"{CounterKey}.{operation}");
}
