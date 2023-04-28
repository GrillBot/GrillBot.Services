using GrillBot.Core.Managers.Performance;
using PointsService.BackgroundServices.PostProcessAction;
using PointsService.Core.Repository;

namespace PointsService.BackgroundServices;

public class PostProcessingService : BackgroundService
{
    private IServiceProvider ServiceProvider { get; }
    private PostProcessingQueue Queue { get; }
    private ICounterManager CounterManager { get; }

    public PostProcessingService(IServiceProvider serviceProvider)
    {
        Queue = serviceProvider.GetRequiredService<PostProcessingQueue>();
        CounterManager = serviceProvider.GetRequiredService<ICounterManager>();
        ServiceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            var request = await Queue.ReadRequestAsync(stoppingToken);

            using var scope = ServiceProvider.CreateScope();
            await ProcessRequestAsync(scope, request);
        }
    }

    private async Task InitAsync()
    {
        using var scope = ServiceProvider.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<PointsServiceRepository>();
        var pendingMigration = await repository.IsPendingMigrationsAsync();
        while (pendingMigration)
            pendingMigration = await repository.IsPendingMigrationsAsync();

        var actions = scope.ServiceProvider.GetServices<PostProcessActionBase>().ToList();
        var users = await repository.User.GetUsersAsync();

        var now = DateTime.UtcNow;
        foreach (var action in actions)
        {
            foreach (var user in users)
            {
                using (CounterManager.Create($"BackgroundService.{action.GetType().Name}"))
                {
                    await action
                        .SetParameters(user, now)
                        .ProcessAsync();
                }
            }
        }

        await repository.CommitAsync();
    }

    private async Task ProcessRequestAsync(IServiceScope scope, PostProcessRequest request)
    {
        var actions = scope.ServiceProvider.GetServices<PostProcessActionBase>().ToList();
        var now = DateTime.UtcNow;

        foreach (var action in actions)
        {
            using (CounterManager.Create($"BackgroundService.{action.GetType().Name}"))
            {
                await action
                    .SetParameters(request, now)
                    .ProcessAsync();
            }
        }
    }
}
