using Discord.Net;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Models;

namespace UnverifyService.Handlers;

public partial class SetUnverifyHandler
{
    private async Task ProcessUnverifySetAsync(UnverifySession session, UnverifyLogItem logItem, CancellationToken cancellationToken)
    {
        if (session.MutedRole is not null && !session.TargetUser.RoleIds.Any(o => o == session.MutedRole.Id))
            await session.TargetUser.AddRoleAsync(session.MutedRole, new() { CancelToken = cancellationToken });

        await session.TargetUser.RemoveRolesAsync(session.RolesToRemove, new() { CancelToken = cancellationToken });

        foreach (var channel in session.ChannelsToRemove)
            await channel.Item1.RemovePermissionOverwriteAsync(session.TargetUser, new() { CancelToken = cancellationToken });

        var activeUnverify = new ActiveUnverify
        {
            EndAtUtc = session.EndAtUtc,
            Id = Guid.NewGuid(),
            StartAtUtc = session.StartAtUtc,
            LogItem = logItem,
            LogSetId = logItem.Id
        };

        await DbContext.AddAsync(activeUnverify, cancellationToken);
        await ContextHelper.SaveChangesAsync(cancellationToken);
    }

    private static async Task RollbackAccessAsync(UnverifySession session, CancellationToken cancellationToken = default)
    {
        await session.TargetUser.AddRolesAsync(session.RolesToRemove, options: new() { CancelToken = cancellationToken });

        foreach (var channel in session.ChannelsToRemove)
            await channel.Item1.AddPermissionOverwriteAsync(session.TargetUser, channel.Item2, new() { CancelToken = cancellationToken });

        if (!session.KeepMutedRole && session.MutedRole is not null)
        {
            try
            {
                await session.TargetUser.RemoveRoleAsync(session.MutedRole, new() { CancelToken = cancellationToken });
            }
            catch (HttpException)
            {
            }
        }
    }
}
