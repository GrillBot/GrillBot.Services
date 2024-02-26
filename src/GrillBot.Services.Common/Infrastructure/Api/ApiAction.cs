using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;

namespace GrillBot.Services.Common.Infrastructure.Api;

public abstract class ApiAction : ApiActionBase
{
    private ICounterManager CounterManager { get; }

    protected ApiAction(ICounterManager counterManager)
    {
        CounterManager = counterManager;
    }

    public CounterItem CreateCounter(string operation)
    {
        var actionName = GetType().Name;
        if (actionName.EndsWith("Action"))
            actionName = actionName[..^"Action".Length];

        return CounterManager.Create($"Api.{actionName}.{operation}");
    }
}
