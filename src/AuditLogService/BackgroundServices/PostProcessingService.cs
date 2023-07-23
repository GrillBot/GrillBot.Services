using AuditLogService.Actions;
using AuditLogService.BackgroundServices.Actions;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Request.CreateItems;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

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
            var logItem = await ReadLogItemToProcessAsync(stoppingToken);
            if (logItem is null)
            {
                await Task.Delay(10000, stoppingToken);
                continue;
            }

            using var scope = ServiceProvider.CreateScope();
            var actions = scope.ServiceProvider.GetServices<PostProcessActionBase>();

            foreach (var action in actions)
                await ProcessActionAsync(action, logItem, scope);
            await FinishLogItemProcessingAsync(logItem, stoppingToken);
        }
    }

    private async Task ProcessActionAsync(PostProcessActionBase action, LogItem logItem, IServiceScope scope)
    {
        var actionName = action.GetType().Name;

        try
        {
            if (!action.CanProcess(logItem))
                return;

            using (CounterManager.Create($"BackgroundService.{actionName}"))
            {
                await action.ProcessAsync(logItem);
            }
        }
        catch (Exception ex)
        {
            await WriteExceptionAsync(ex, logItem, scope, actionName);
        }
    }

    private async Task WriteExceptionAsync(Exception exception, LogItem logItem, IServiceScope scope, string actionName)
    {
        var source = $"{nameof(PostProcessingService)}/{actionName}";
        var logMessage = new Discord.LogMessage(Discord.LogSeverity.Error, source, $"An error occured while processing log item ID {logItem.Id}", exception)
            .ToString(prependTimestamp: false, padSource: 50);

        var request = new LogRequest
        {
            CreatedAt = DateTime.UtcNow,
            LogMessage = new LogMessageRequest
            {
                Message = logMessage,
                Severity = Discord.LogSeverity.Error,
                Source = source,
                SourceAppName = "AuditLogService"
            },
            Type = Core.Enums.LogType.Error
        };

        var createAction = scope.ServiceProvider.GetRequiredService<CreateItemsAction>();
        createAction.Init(new DefaultHttpContext(), new object[] { new List<LogRequest> { request } });
        await createAction.ProcessAsync();
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
            .First(o => o is ComputeInteractionStatisticsAction);

        var logItems = await context.LogItems.Where(o => o.Type == Core.Enums.LogType.InteractionCommand).Include(o => o.InteractionCommand).ToListAsync();
        var groupedItems = logItems
            .Where(o => o.InteractionCommand is not null)
            .GroupBy(o => $"{o.InteractionCommand!.Name} ({o.InteractionCommand.ModuleName}/{o.InteractionCommand.MethodName})")
            .Select(o => o.First())
            .ToList();

        foreach (var logItem in groupedItems)
        {
            await action.ProcessAsync(logItem);
        }
    }
}
