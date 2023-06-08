namespace AuditLogService.Actions;

public static class ActionExtensions
{
    public static void AddActions(this IServiceCollection services)
    {
        services
            .AddScoped<CreateItemsAction>()
            .AddScoped<DeleteItemAction>();
    }
}
