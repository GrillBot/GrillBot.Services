using AuditLogService.BackgroundServices.Actions;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices;

public partial class PostProcessingService : BackgroundService
{
    private IServiceProvider ServiceProvider { get; }
    private ICounterManager CounterManager { get; }

    public PostProcessingService(IServiceProvider serviceProvider)
    {
        CounterManager = serviceProvider.GetRequiredService<ICounterManager>();
        ServiceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await CheckMigrationsAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            var logItems = await ReadItemsToProcessAsync(stoppingToken);
            foreach (var item in logItems)
            {
                await ResetProcessFlagAsync(item);

                using (CounterManager.Create("BackgroundService.LogItems.Process"))
                {
                    using var scope = ServiceProvider.CreateScope();
                    var actions = scope.ServiceProvider.GetServices<PostProcessActionBase>();

                    foreach (var action in actions)
                        await ProcessActionAsync(action, item, scope);
                }
            }

            await Task.Delay(10000, stoppingToken);
        }
    }

    private async Task ProcessActionAsync(PostProcessActionBase action, LogItem logItem, IServiceScope scope)
    {
        const string actionSuffix = "Action";
        var actionName = action.GetType().Name;
        if (actionName.EndsWith(actionSuffix))
            actionName = actionName[..^actionSuffix.Length];

        try
        {
            if (!action.CanProcess(logItem))
                return;

            using (CounterManager.Create($"BackgroundService.{actionName}"))
            {
                await action.ProcessAsync(logItem);
            }
        }
        catch (Exception)
        {
        }
    }

    private async Task CheckMigrationsAsync()
    {
        using var scope = ServiceProvider.CreateScope();

        await CheckMigrationsAsync<AuditLogServiceContext>(scope);
        await CheckMigrationsAsync<AuditLogStatisticsContext>(scope);
    }

    private static async Task CheckMigrationsAsync<TContext>(IServiceScope scope) where TContext : DbContext
    {
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).Any();
        while (pendingMigrations)
            pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).Any();
    }
}
