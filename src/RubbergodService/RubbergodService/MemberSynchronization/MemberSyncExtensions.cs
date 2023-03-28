namespace RubbergodService.MemberSynchronization;

public static class MemberSyncExtensions
{
    public static IServiceCollection AddMemberSync(this IServiceCollection services)
    {
        return services
            .AddSingleton<MemberSyncQueue>()
            .AddHostedService<MemberSyncService>();
    }
}
