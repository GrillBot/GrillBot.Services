using AuditLogService.BackgroundServices.Actions;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace AuditLogService.BackgroundServices;

public class PostProcessingService : BackgroundService
{
    private IServiceProvider ServiceProvider { get; }
    private ICounterManager CounterManager { get; }
    private Channel<LogItem> Channel { get; }

    public PostProcessingService(IServiceProvider serviceProvider)
    {
        CounterManager = serviceProvider.GetRequiredService<ICounterManager>();
        ServiceProvider = serviceProvider;
        Channel = serviceProvider.GetRequiredService<Channel<LogItem>>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await CheckMigrationsAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            var logItem = await Channel.Reader.ReadAsync(stoppingToken);

            using var scope = ServiceProvider.CreateScope();
            var actions = scope.ServiceProvider.GetServices<PostProcessActionBase>();

            foreach (var action in actions.Where(a => a.CanProcess(logItem)))
            {
                using (CounterManager.Create($"BackgroundService.{action.GetType().Name}"))
                {
                    await action.ProcessAsync(logItem);
                }
            }
        }
    }

    private async Task CheckMigrationsAsync()
    {
        using var scope = ServiceProvider.CreateScope();

        await CheckMigrationsAsync<AuditLogServiceContext>(scope);
        await CheckMigrationsAsync<AuditLogStatisticsContext>(scope);
        //await MigrateDataAsync(scope);
    }

    private static async Task CheckMigrationsAsync<TContext>(IServiceScope scope) where TContext : DbContext
    {
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).Any();
        while (pendingMigrations)
            pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).Any();
    }

    private async Task MigrateDataAsync(IServiceScope scope)
    {
        var context = scope.ServiceProvider.GetRequiredService<AuditLogServiceContext>();
        var action = scope.ServiceProvider.GetServices<PostProcessActionBase>()
            .First(o => o is ComputeDateStatisticsAction);

        var logItems = await context.LogItems.ToListAsync();
        var groupedItems = logItems
            .GroupBy(o => o.CreatedAt.Date)
            .Select(o => o.First())
            .ToList();

        foreach (var logItem in groupedItems)
        {
            await action.ProcessAsync(logItem);
        }
    }
}
