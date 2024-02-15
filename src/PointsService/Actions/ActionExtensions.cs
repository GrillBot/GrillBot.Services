namespace PointsService.Actions;

public static class ActionExtensions
{
    public static void AddActions(this IServiceCollection services)
    {
        // Merge
        services
            .AddScoped<Merge.MergeValidTransactionsAction>();

        // Users
        services
            .AddScoped<Users.UserListAction>();

        services
            .AddScoped<CurrentPointsStatusAction>()
            .AddScoped<LeaderboardAction>()
            .AddScoped<ChartAction>()
            .AddScoped<AdminListAction>()
            .AddScoped<TransferPointsAction>()
            .AddScoped<TransactionExistsAction>()
            .AddScoped<ImagePointsStatusAction>()
            .AddScoped<LeaderboardCountAction>()
            .AddScoped<GetStatusInfoAction>();
    }
}
