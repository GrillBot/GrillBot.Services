using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.EntityFramework.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GrillBot.Services.Common.Infrastructure.Api;

public abstract class ApiAction<TParentAction, TDbContext>(IServiceProvider serviceProvider) : ApiAction<TDbContext>(serviceProvider)
    where TDbContext : DbContext
    where TParentAction : ApiActionBase
{
    protected TParentAction ParentApiAction { get; } = serviceProvider.GetRequiredService<TParentAction>();

    protected Task<ApiResult> ExecuteParentAction(object?[]? parameters = null)
    {
        parameters ??= Parameters;
        ParentApiAction.Init(HttpContext, parameters, CurrentUser);

        return ParentApiAction.ProcessAsync();
    }
}

public abstract class ApiAction<TDbContext> : ApiAction where TDbContext : DbContext
{
    protected ContextHelper<TDbContext> ContextHelper { get; }
    protected TDbContext DbContext => ContextHelper.DbContext;

    protected ApiAction(ICounterManager counterManager, TDbContext dbContext) : base(counterManager)
    {
        ContextHelper = new ContextHelper<TDbContext>(counterManager, dbContext, CounterKey);
    }

    protected ApiAction(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<TDbContext>();
        ContextHelper = new ContextHelper<TDbContext>(CounterManager, dbContext, CounterKey);
    }
}

public abstract class ApiAction : ApiActionBase
{
    protected ICounterManager CounterManager { get; }

    protected string CounterKey { get; }

    protected ApiAction(ICounterManager counterManager)
    {
        CounterManager = counterManager;
        CounterKey = CreateCounterKey();
    }

    protected ApiAction(IServiceProvider serviceProvider) : this(serviceProvider.GetRequiredService<ICounterManager>())
    {
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
