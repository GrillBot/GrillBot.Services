using Discord;
using GrillBot.Core.Extensions;
using RubbergodService.Core.Entity;
using RubbergodService.Core.Repository;
using RubbergodService.Discord;

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

            var repository = scope.ServiceProvider.GetRequiredService<RubbergodServiceRepository>();
            var discordManager = scope.ServiceProvider.GetRequiredService<DiscordManager>();
            await ProcessAsync(repository, discordManager, memberId);

            Logger.LogInformation("Processing synchronization of member {memberId} finished", memberId);
        }
    }

    private static async Task ProcessAsync(RubbergodServiceRepository repository, DiscordManager discordManager, string memberId)
    {
        var member = await repository.MemberCache.FindMemberByIdAsync(memberId);
        if (member is null)
        {
            member = new MemberCacheItem { UserId = memberId };
            await repository.AddAsync(member);
        }

        var user = await discordManager.GetUserAsync(memberId.ToUlong());
        if (user is null)
        {
            member.Username = "Deleted user";
            member.AvatarUrl = CDN.GetDefaultUserAvatarUrl(0);
        }
        else
        {
            member.Username = user.Username;
            member.AvatarUrl = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();
        }

        await repository.CommitAsync();
    }
}
