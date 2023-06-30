using GrillBot.Core.Managers.Performance;
using PointsService.BackgroundServices.Actions;
using PointsService.Core.Entity;
using PointsService.Core.Repository;

namespace PointsService.BackgroundServices;

public class PostProcessingService : BackgroundService
{
    private IServiceProvider ServiceProvider { get; }
    private ICounterManager CounterManager { get; }

    private bool IsCheckedMigrations { get; set; }

    public PostProcessingService(IServiceProvider serviceProvider)
    {
        CounterManager = serviceProvider.GetRequiredService<ICounterManager>();
        ServiceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var user = await FindPendingUserAsync();
            if (user is null)
            {
                await Task.Delay(30000, stoppingToken);
                continue;
            }

            using var scope = ServiceProvider.CreateScope();
            var actions = scope.ServiceProvider.GetServices<PostProcessActionBase>();
            await ResetPendingStateAsync(scope, user);

            foreach (var action in actions)
            {
                using (CounterManager.Create($"BackgroundService.{action.GetType().Name}"))
                {
                    await action.ProcessAsync(user);
                }
            }
        }
    }

    private async Task<User?> FindPendingUserAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<PointsServiceRepository>();

        if (IsCheckedMigrations)
            return await repository.User.FindFirstUserForPostProcessing();

        var pendingMigration = await repository.IsPendingMigrationsAsync();
        while (pendingMigration)
            pendingMigration = await repository.IsPendingMigrationsAsync();
        IsCheckedMigrations = true;

        return await repository.User.FindFirstUserForPostProcessing();
    }

    private static async Task ResetPendingStateAsync(IServiceScope scope, User user)
    {
        var repository = scope.ServiceProvider.GetRequiredService<PointsServiceRepository>();

        var entity = await repository.User.FindUserAsync(user.GuildId, user.Id);
        entity!.PendingRecalculation = false;

        await repository.CommitAsync();
    }
}
