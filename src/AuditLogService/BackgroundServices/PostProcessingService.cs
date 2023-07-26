using AuditLogService.Actions;
using AuditLogService.BackgroundServices.Actions;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Request.CreateItems;
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

                using var scope = ServiceProvider.CreateScope();
                var actions = scope.ServiceProvider.GetServices<PostProcessActionBase>();
                foreach (var action in actions)
                    await ProcessActionAsync(action, item, scope);
            }

            await Task.Delay(10000, stoppingToken);
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

        async Task<List<ApiRequest>> ReadLogRequestsAsync(DateTime lastRequest)
            => await context.ApiRequests.Where(o => o.ForwardedIp == null && o.EndAt > lastRequest).Take(1000).ToListAsync();

        var lastRequest = DateTime.MinValue;
        var items = await ReadLogRequestsAsync(lastRequest);
        while (items.Count > 0)
        {
            foreach (var item in items)
            {
                if (item.Headers.TryGetValue("X-Forwarded-For", out var forwardedIp))
                    item.ForwardedIp = forwardedIp;

                lastRequest = item.EndAt;
            }

            await context.SaveChangesAsync();
            items = await ReadLogRequestsAsync(lastRequest);
        }
    }
}
