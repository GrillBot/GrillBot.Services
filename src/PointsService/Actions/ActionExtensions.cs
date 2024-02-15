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
            .AddScoped<Users.GetUserListAction>();

        services
            .AddScoped<GetCurrentPointsStatusAction>()
            .AddScoped<GetLeaderboardAction>()
            .AddScoped<GetChartAction>()
            .AddScoped<GetAdminListAction>()
            .AddScoped<ProcessTransferPointsAction>()
            .AddScoped<CheckTransactionExistsAction>()
            .AddScoped<GetImagePointsStatusAction>()
            .AddScoped<GetLeaderboardCountAction>()
            .AddScoped<GetStatusInfoAction>();
    }
}
