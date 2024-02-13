namespace PointsService.Actions;

public static class ActionExtensions
{
    public static void AddActions(this IServiceCollection services)
    {
        // Users
        services
            .AddScoped<Users.UserListAction>();

        services
            .AddScoped<SynchronizationAction>()
            .AddScoped<CurrentPointsStatusAction>()
            .AddScoped<LeaderboardAction>()
            .AddScoped<ChartAction>()
            .AddScoped<AdminListAction>()
            .AddScoped<MergeTransactionsAction>()
            .AddScoped<TransferPointsAction>()
            .AddScoped<TransactionExistsAction>()
            .AddScoped<ImagePointsStatusAction>()
            .AddScoped<LeaderboardCountAction>()
            .AddScoped<GetStatusInfoAction>();
    }
}
