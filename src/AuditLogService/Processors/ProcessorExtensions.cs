using System.Reflection;
using AuditLogService.Processors.Request;

namespace AuditLogService.Processors;

public static class ProcessorExtensions
{
    public static void AddProcessors(this IServiceCollection services)
    {
        var baseType = typeof(RequestProcessorBase);
        
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes()
            .Where(o => baseType.IsAssignableFrom(o) && !o.IsAbstract);
        foreach (var type in types)
            services.AddScoped(baseType, type);
    }
}
