using GrillBot.Core.Infrastructure.Actions;

namespace EmoteService.Actions;

public static class ActionExtensions
{
    public static void AddActions(this IServiceCollection services)
    {
        var assembly = typeof(ActionExtensions).Assembly;
        var apiActionType = typeof(ApiActionBase);
        var types = assembly.GetTypes();

        foreach (var action in types.Where(o => !o.IsAbstract && o.IsClass && apiActionType.IsAssignableFrom(o)))
            services.AddScoped(action);
    }
}
