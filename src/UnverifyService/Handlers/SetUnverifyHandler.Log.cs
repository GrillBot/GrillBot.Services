using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Services.UserMeasures.Models.Events;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Core.Enums;
using UnverifyService.Models;

namespace UnverifyService.Handlers;

public partial class SetUnverifyHandler
{
    private async Task<UnverifyLogItem> LogUnverifyAsync(UnverifySession session, ICurrentUserProvider currentUser, CancellationToken cancellationToken = default)
    {
        var entity = new UnverifyLogItem
        {
            CreatedAt = DateTime.UtcNow,
            FromUserId = currentUser.Id.ToUlong(),
            GuildId = session.TargetUser.GuildId,
            Id = Guid.NewGuid(),
            OperationType = session.IsSelfUnverify ? UnverifyOperationType.SelfUnverify : UnverifyOperationType.Unverify,
            ToUserId = session.TargetUser.Id,
            SetOperation = new UnverifyLogSetOperation
            {
                Channels = [
                    .. session.ChannelsToRemove.Select(o => new UnverifyLogSetChannel
                    {
                        AllowValue = o.Item2.AllowValue,
                        ChannelId = o.Item1.Id,
                        DenyValue = o.Item2.DenyValue,
                        IsKept = false
                    }),
                    .. session.ChannelsToKeep.Select(o => new UnverifyLogSetChannel
                    {
                        AllowValue = o.Item2.AllowValue,
                        ChannelId = o.Item1.Id,
                        DenyValue = o.Item2.DenyValue,
                        IsKept = true
                    })
                ],
                EndAtUtc = session.EndAtUtc,
                KeepMutedRole = session.KeepMutedRole,
                Language = session.TargetUserEntity?.Language ?? "en-US",
                Reason = session.Reason,
                StartAtUtc = session.StartAtUtc,
                Roles = [
                    .. session.RolesToRemove.Select(o => new UnverifyLogSetRole
                    {
                        IsKept = false,
                        RoleId = o.Id
                    }),
                    .. session.RolesToKeep.Select(o => new UnverifyLogSetRole
                    {
                        IsKept = true,
                        RoleId = o.Id
                    })
                ]
            }
        };

        await ContextHelper.DbContext.AddAsync(entity, cancellationToken);
        await ContextHelper.SaveChangesAsync(cancellationToken);

        return entity;
    }

    private Task NotifyUserMeasuresAsync(UnverifySession session, UnverifyLogItem logItem, ICurrentUserProvider currentUser, CancellationToken cancellationToken = default)
    {
        if (session.IsSelfUnverify)
            return Task.CompletedTask;

        var payload = new UnverifyPayload(
            session.StartAtUtc,
            session.Reason ?? "",
            session.TargetUser.GuildId.ToString(),
            currentUser.Id!,
            session.TargetUser.Id.ToString(),
            session.EndAtUtc,
            logItem.LogNumber
        );

        return Publisher.PublishAsync(payload, cancellationToken: cancellationToken);
    }
}
