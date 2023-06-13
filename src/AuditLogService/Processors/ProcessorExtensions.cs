namespace AuditLogService.Processors;

public static class ProcessorExtensions
{
    public static void AddProcessors(this IServiceCollection services)
    {
        services
            .AddScoped<RequestProcessorFactory>();
    }
}
