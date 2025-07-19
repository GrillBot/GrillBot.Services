using System.Linq.Expressions;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Core.Enums;
using Response = UnverifyService.Models.Response.Logs;

namespace UnverifyService.Actions.Logs;

public partial class GetUnverifyLogsAction
{
    private async Task<Response.UnverifyLogItem> MapItemAsync(UnverifyLogItem item)
    {
        object? preview = item.OperationType switch
        {
            UnverifyOperationType.Unverify => await CreateUnverifyPreviewAsync(item),
            UnverifyOperationType.SelfUnverify => await CreateSelfUnverifyPreviewAsync(item),
            UnverifyOperationType.AutoRemove => await CreateAutoRemovePreviewAsync(item),
            UnverifyOperationType.ManualRemove => await CreateManualRemovePreviewAsync(item),
            UnverifyOperationType.Update => await CreateUpdatePreviewAsync(item),
            UnverifyOperationType.Recovery => await CreateRecoveryPreviewAsync(item),
            _ => null
        };

        return new Response.UnverifyLogItem(
            item.Id,
            item.ParentLogItemId,
            item.OperationType,
            item.GuildId.ToString(),
            item.FromUserId.ToString(),
            item.ToUserId.ToString(),
            item.CreatedAt,
            preview
        );
    }

    private Task<Response.UnverifyPreview?> CreateUnverifyPreviewAsync(UnverifyLogItem item)
    {
        return CreatePreviewAsync<UnverifyLogSetOperation, Response.UnverifyPreview>(item, entity => new Response.UnverifyPreview(
            entity.StartAtUtc,
            entity.EndAtUtc,
            entity.Roles.Count(o => !o.IsKept),
            entity.Roles.Count(o => o.IsKept),
            entity.Channels.Count(o => !o.IsKept),
            entity.Channels.Count(o => o.IsKept),
            entity.Reason ?? ""
        ));
    }

    private Task<Response.SelfUnverifyPreview?> CreateSelfUnverifyPreviewAsync(UnverifyLogItem item)
    {
        return CreatePreviewAsync<UnverifyLogSetOperation, Response.SelfUnverifyPreview>(item, entity => new Response.SelfUnverifyPreview(
            entity.StartAtUtc,
            entity.EndAtUtc,
            entity.Roles.Count(o => !o.IsKept),
            entity.Roles.Count(o => o.IsKept),
            entity.Channels.Count(o => !o.IsKept),
            entity.Channels.Count(o => o.IsKept)
        ));
    }

    private Task<Response.AutoRemovePreview?> CreateAutoRemovePreviewAsync(UnverifyLogItem item)
    {
        return CreatePreviewAsync<UnverifyLogRemoveOperation, Response.AutoRemovePreview>(
            item,
            entity => new Response.AutoRemovePreview(entity.Roles.Count, entity.Channels.Count)
        );
    }

    private Task<Response.RemovePreview?> CreateManualRemovePreviewAsync(UnverifyLogItem item)
    {
        return CreatePreviewAsync<UnverifyLogRemoveOperation, Response.RemovePreview>(item, entity => new Response.RemovePreview(
            entity.Roles.Count,
            entity.Channels.Count,
            entity.IsFromWeb,
            entity.Force
        ));
    }

    private Task<Response.UpdatePreview?> CreateUpdatePreviewAsync(UnverifyLogItem item)
    {
        return CreatePreviewAsync<UnverifyLogUpdateOperation, Response.UpdatePreview>(item, entity => new Response.UpdatePreview(
            entity.NewStartAtUtc,
            entity.NewEndAtUtc,
            entity.Reason
        ));
    }

    private Task<Response.RecoveryPreview?> CreateRecoveryPreviewAsync(UnverifyLogItem item)
    {
        return CreatePreviewAsync<UnverifyLogRemoveOperation, Response.RecoveryPreview>(
            item,
            entity => new Response.RecoveryPreview(entity.Roles.Count, entity.Channels.Count)
        );
    }

    private async Task<TPreview?> CreatePreviewAsync<TEntity, TPreview>(UnverifyLogItem item, Expression<Func<TEntity, TPreview>> projection) where TEntity : UnverifyOperationBase
    {
        var query = DbContext.Set<TEntity>()
            .Where(o => o.LogItemId == item.Id)
            .Select(projection);

        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query, CancellationToken);
    }
}
