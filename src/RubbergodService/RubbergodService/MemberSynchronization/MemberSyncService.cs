using RubbergodService.Managers;

namespace RubbergodService.MemberSynchronization;

public class MemberSyncService : BackgroundService
{
    private MemberSyncQueue Queue { get; }
    private IServiceProvider ServiceProvider { get; }
    private ILogger<MemberSyncService> Logger { get; }

    public MemberSyncService(MemberSyncQueue queue, IServiceProvider provider, ILogger<MemberSyncService> logger)
    {
        Queue = queue;
        ServiceProvider = provider;
        Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var memberId = await Queue.DequeueAsync(stoppingToken);
            Logger.LogInformation("Processing synchronization of member {memberId} started", memberId);

            using var scope = ServiceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<UserManager>().InitMemberAsync(memberId);
            
            Logger.LogInformation("Processing synchronization of member {memberId} finished", memberId);
        }
    }
}
