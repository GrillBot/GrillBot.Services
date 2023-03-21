namespace PointsService.Actions;

public static class ActionExtensions
{
    public static IServiceCollection AddActions(this IServiceCollection services)
    {
        return services
            .AddScoped<CreateTransactionAction>()
            .AddScoped<DeleteTransactionsAction>();
    }
}
