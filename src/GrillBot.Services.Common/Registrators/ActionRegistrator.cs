using GrillBot.Core.Infrastructure.Actions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GrillBot.Services.Common.Registrators;

public static class ActionRegistrator
{
    private static readonly Type _apiActionType = typeof(ApiActionBase);

    public static void RegisterActionsFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var types = assembly.GetTypes();

        foreach (var type in types.Where(o => o.IsClass && !o.IsAbstract && _apiActionType.IsAssignableFrom(o)))
            services.Add(ServiceDescriptor.Describe(type, type, lifetime));
    }
}
