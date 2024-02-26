namespace AuditLogService.Managers;

public static class ManagerExtensions
{
    public static void AddManagers(this IServiceCollection services)
    {
        services
            .AddScoped<DataRecalculationManager>();
    }
}
