using Discord;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Models.Response.Logs.Detail;

namespace UnverifyService.Actions.Logs;

public partial class GetUnverifyLogDetailAction
{
    private async Task<UnverityOperationDetailData?> CreateUnverifyOperationDetailDataAsync(Guid logItemId)
    {
        var dataQuery = CreateDetailQuery<UnverifyLogSetOperation>(logItemId, o => o.Include(x => x.Roles).Include(x => x.Channels));
        var data = await ContextHelper.ReadFirstOrDefaultEntityAsync(dataQuery, CancellationToken);
        if (data is null)
            return null;

        var removedChannels = new List<ChannelDetailData>();
        var keepedChannels = new List<ChannelDetailData>();
        foreach (var channel in data.Channels)
        {
            var channelData = new ChannelDetailData(
                channel.ChannelId.ToString(),
                [.. channel.Perms.ToAllowList().Select(o => o.ToString())],
                [.. channel.Perms.ToDenyList().Select(o => o.ToString())]
            );

            if (channel.IsKept)
                keepedChannels.Add(channelData);
            else
                removedChannels.Add(channelData);
        }

        return new UnverityOperationDetailData(
            data.StartAtUtc,
            data.EndAtUtc,
            (long)Math.Ceiling((data.EndAtUtc - data.StartAtUtc).TotalMilliseconds),
            data.Reason ?? "",
            data.Language,
            data.KeepMutedRole,
            [.. data.Roles.Where(o => !o.IsKept).Select(o => o.RoleId.ToString())],
            [.. data.Roles.Where(o => o.IsKept).Select(o => o.RoleId.ToString())],
            removedChannels,
            keepedChannels
        );
    }

    private async Task<SelfUnverifyOperationDetailData?> CreateSelfUnverifyOperationDetailDataAsync(Guid logItemId)
    {
        var unverifyData = await CreateUnverifyOperationDetailDataAsync(logItemId);
        if (unverifyData is null)
            return null;

        return new SelfUnverifyOperationDetailData(
            unverifyData.StartAtUtc,
            unverifyData.EndAtUtc,
            unverifyData.DurationMilliseconds,
            unverifyData.Language,
            unverifyData.IsMutedRoleKeeped,
            unverifyData.RemovedRoles,
            unverifyData.KeepedRoles,
            unverifyData.RemovedChannels,
            unverifyData.KeepedChannels
        );
    }

    private async Task<ManualRemoveOperationDetailData?> CreateManualRemoveOperationDetailDataAsync(Guid logItemId)
    {
        var dataQuery = CreateDetailQuery<UnverifyLogRemoveOperation>(logItemId, o => o.Include(x => x.Roles).Include(x => x.Channels));
        var data = await ContextHelper.ReadFirstOrDefaultEntityAsync(dataQuery, CancellationToken);
        if (data is null)
            return null;

        return new ManualRemoveOperationDetailData(
            data.IsFromWeb,
            data.Language ?? "",
            data.Force,
            [.. data.Roles.Select(o => o.RoleId.ToString())],
            [..
                data.Channels.Select(o => new ChannelDetailData(
                    o.ChannelId.ToString(),
                    [.. o.Perms.ToAllowList().Select(o => o.ToString())],
                    [.. o.Perms.ToDenyList().Select(o => o.ToString())]
                ))
            ]
        );
    }

    private async Task<AutoRemoveOperationDetailData?> CreateAutoRemoveOperationDetailDataAsync(Guid logItemId)
    {
        var data = await CreateManualRemoveOperationDetailDataAsync(logItemId);
        return data is null ? null : new AutoRemoveOperationDetailData(data.Language, data.ReturnedRoles, data.ReturnedChannels);
    }

    private async Task<UpdateOperationDetailData?> CreateUpdateOperationDetailDataAsync(Guid logItemId)
    {
        var query = CreateDetailQuery<UnverifyLogUpdateOperation>(logItemId)
            .Select(o => new UpdateOperationDetailData(
                o.NewStartAtUtc,
                o.NewEndAtUtc,
                (long)Math.Ceiling((o.NewEndAtUtc - o.NewStartAtUtc).TotalMilliseconds),
                o.LogItem.ParentLogItem!.SetOperation!.StartAtUtc,
                o.LogItem.ParentLogItem!.SetOperation!.EndAtUtc,
                (long)Math.Ceiling((o.LogItem.ParentLogItem!.SetOperation!.EndAtUtc - o.LogItem.ParentLogItem!.SetOperation!.StartAtUtc).TotalMilliseconds),
                o.Reason ?? ""
            ));

        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query, CancellationToken);
    }

    private async Task<RecoveryOperationDetailData?> CreateRecoveryOperationDetailDataAsync(Guid logItemId)
    {
        var query = CreateDetailQuery<UnverifyLogRemoveOperation>(logItemId, o => o.Include(x => x.Roles).Include(x => x.Channels));
        var data = await ContextHelper.ReadFirstOrDefaultEntityAsync(query, CancellationToken);
        if (data is null)
            return null;

        return new RecoveryOperationDetailData(
            [.. data.Roles.Select(o => o.RoleId.ToString())],
            [..
                data.Channels.Select(o => new ChannelDetailData(
                    o.ChannelId.ToString(),
                    [.. o.Perms.ToAllowList().Select(o => o.ToString())],
                    [.. o.Perms.ToDenyList().Select(o => o.ToString())]
                ))
            ]
        );
    }

    private IQueryable<TEntity> CreateDetailQuery<TEntity>(Guid logItemId, Func<IQueryable<TEntity>, IQueryable<TEntity>>? joinsAndFilters = null) where TEntity : UnverifyOperationBase
    {
        var query = DbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(o => o.LogItemId == logItemId);

        return joinsAndFilters is not null ? joinsAndFilters(query) : query;
    }
}
