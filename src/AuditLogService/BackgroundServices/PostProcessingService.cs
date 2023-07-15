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
        //await ComputeOldStatisticsAsync();

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
    }

    private static async Task CheckMigrationsAsync<TContext>(IServiceScope scope) where TContext : DbContext
    {
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).Any();
        while (pendingMigrations)
            pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).Any();
    }

    private async Task ComputeOldStatisticsAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditLogServiceContext>();
        var actions = scope.ServiceProvider.GetServices<PostProcessActionBase>();

        var logItems = await context.LogItems.Where(o => o.Type == Core.Enums.LogType.Api).AsNoTracking()
            .Include(o => o.ApiRequest).ToListAsync();
        logItems = logItems.Where(o => o.ApiRequest is not null).ToList();

        var perDate = logItems.GroupBy(o => o.CreatedAt.Date).Select(o => o.First()).ToList();
        var perResult = logItems.GroupBy(o => o.ApiRequest!.Result).Select(o => o.First()).ToList();
        var perEndpoint = logItems.GroupBy(o => $"{o.ApiRequest!.Method} {o.ApiRequest.TemplatePath}").Select(o => o.First()).ToList();

        foreach (var item in perDate)
        {
            foreach (var action in actions.Where(o => o is ComputeApiDateCountsAction or ComputeAvgTimesAction or DeleteInvalidStatisticsAction))
            {
                await action.ProcessAsync(item);
            }
        }

        foreach (var item in perResult)
        {
            foreach (var action in actions.Where(a => a is ComputeApiResultCountsAction or DeleteInvalidStatisticsAction))
            {
                await action.ProcessAsync(item);
            }
        }

        foreach (var item in perEndpoint)
        {
            foreach (var action in actions.Where(a => a is ComputeApiRequestStatsAction or DeleteInvalidStatisticsAction))
            {
                await action.ProcessAsync(item);
            }
        }

        logItems = await context.LogItems.Where(o => o.Type == Core.Enums.LogType.JobCompleted).AsNoTracking()
            .Include(o => o.Job).ToListAsync();
        logItems = logItems.FindAll(o => o.Job is not null);

        perDate = logItems.GroupBy(o => o.CreatedAt.Date).Select(o => o.First()).ToList();
        foreach (var item in perDate)
        {
            foreach (var action in actions.Where(a => a is ComputeAvgTimesAction or DeleteInvalidStatisticsAction))
            {
                await action.ProcessAsync(item);
            }
        }

        logItems = await context.LogItems.Where(o => o.Type == Core.Enums.LogType.InteractionCommand).AsNoTracking()
            .Include(o => o.InteractionCommand).ToListAsync();
        logItems = logItems.FindAll(o => o.InteractionCommand is not null);

        perDate = logItems.GroupBy(o => o.CreatedAt.Date).Select(o => o.First()).ToList();
        foreach (var item in perDate)
        {
            foreach (var action in actions.Where(a => a is ComputeAvgTimesAction or DeleteInvalidStatisticsAction))
            {
                await action.ProcessAsync(item);
            }
        }
    }
}
