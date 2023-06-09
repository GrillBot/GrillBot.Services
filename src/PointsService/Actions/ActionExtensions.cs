﻿namespace PointsService.Actions;

public static class ActionExtensions
{
    public static void AddActions(this IServiceCollection services)
    {
        services
            .AddScoped<CreateTransactionAction>()
            .AddScoped<DeleteTransactionsAction>()
            .AddScoped<SynchronizationAction>()
            .AddScoped<CurrentPointsStatusAction>()
            .AddScoped<LeaderboardAction>()
            .AddScoped<ChartAction>()
            .AddScoped<AdminListAction>()
            .AddScoped<MergeTransactionsAction>()
            .AddScoped<TransferPointsAction>()
            .AddScoped<AdminCreateTransactionAction>()
            .AddScoped<TransactionExistsAction>()
            .AddScoped<ImagePointsStatusAction>()
            .AddScoped<LeaderboardCountAction>()
            .AddScoped<GetStatusInfoAction>();
    }
}
