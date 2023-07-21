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
        var actions = scope.ServiceProvider.GetServices<PostProcessActionBase>()
            .Where(o => o is ComputeInteractionUserStatisticsAction or DeleteInvalidStatisticsAction)
            .ToArray();

        var logItems = await context.LogItems.Where(o => o.Type == Core.Enums.LogType.InteractionCommand).Include(o => o.InteractionCommand).ToListAsync();
        logItems = logItems.FindAll(o => o.InteractionCommand is not null);

        var groupedItems = logItems
            .GroupBy(o => new { o.UserId, o.InteractionCommand!.Name, o.InteractionCommand.ModuleName, o.InteractionCommand.MethodName })
            .Select(o => o.First())
            .ToList();

        foreach (var logItem in groupedItems)
        {
            foreach (var action in actions)
            {
                await action.ProcessAsync(logItem);
            }
        }
    }
}
