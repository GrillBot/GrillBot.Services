namespace AuditLogService.Actions;

public static class ActionExtensions
{
    public static void AddActions(this IServiceCollection services)
    {
        services
            .AddScoped<CreateItemsAction>()
            .AddScoped<Delete.DeleteItemAction>()
            .AddScoped<Delete.BulkDeleteAction>()
            .AddScoped<Search.SearchItemsAction>()
            .AddScoped<Detail.ReadDetailAction>()
            .AddScoped<Archivation.ArchiveOldLogsAction>()
            .AddScoped<Statistics.GetAuditLogStatisticsAction>()
            .AddScoped<Statistics.GetInteractionStatisticsAction>()
            .AddScoped<Statistics.GetUserCommandStatisticsAction>()
            .AddScoped<Statistics.GetApiStatisticsAction>()
            .AddScoped<Statistics.GetUserApiStatisticsAction>()
            .AddScoped<Statistics.GetAvgTimesAction>()
            .AddScoped<Info.GetJobsInfoAction>()
            .AddScoped<Info.GetItemsCountOfGuildAction>()
            .AddScoped<Dashboard.GetApiDashboardAction>()
            .AddScoped<Dashboard.GetInteractionsDashboardAction>()
            .AddScoped<Dashboard.GetJobsDashboardListAction>()
            .AddScoped<Dashboard.GetTodayAvgTimesDashboard>()
            .AddScoped<Info.GetStatusInfoAction>();
    }
}
