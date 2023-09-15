using GrillBot.Core.Managers.Performance;
using PointsService.BackgroundServices.Actions;
using PointsService.Core.Entity;
using PointsService.Core.Repository;

namespace PointsService.BackgroundServices;

public class PostProcessingService : BackgroundService
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
            using var scope = ServiceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<PointsServiceRepository>();
            var user = await repository.User.FindFirstUserForPostProcessing();

            if (user is null)
            {
                await Task.Delay(30000, stoppingToken);
                continue;
            }

            var actions = scope.ServiceProvider.GetServices<PostProcessActionBase>();
            await ResetPendingStateAsync(repository, user);

            foreach (var action in actions)
                await ProcessActionAsync(action, user);
        }
    }

    private async Task ProcessActionAsync(PostProcessActionBase action, User user)
    {
        const string actionSuffix = "Action";
        var actionName = action.GetType().Name;
        if (actionName.EndsWith(actionSuffix))
            actionName = actionName[..^actionSuffix.Length];

        using (CounterManager.Create($"BackgroundService.{actionName}"))
        {
            await action.ProcessAsync(user);
        }
    }

    private static async Task ResetPendingStateAsync(PointsServiceRepository repository, User user)
    {
        var entity = await repository.User.FindUserAsync(user.GuildId, user.Id);
        entity!.PendingRecalculation = false;

        await repository.CommitAsync();
    }

    private async Task CheckMigrationsAsync()
    {
        using var scope = ServiceProvider.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<PointsServiceRepository>();
        var pendingMigrations = await repository.IsPendingMigrationsAsync();
        while (pendingMigrations)
            pendingMigrations = await repository.IsPendingMigrationsAsync();
    }
}
