using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Core.Enums;

namespace UnverifyService.Actions.Archivation;

public partial class CreateArchivationDataAction
{
    public Task<int> CountAsync()
        => ContextHelper.ReadCountAsync(CreateQuery(), CancellationToken);

    private async Task<bool> ExistsItemtoArchiveAsync()
    {
        var count = await CountAsync();
        return count >= _options.Value.Archivation.MinimalCount;
    }

    private async Task<List<UnverifyLogItem>> ReadItemsToArchiveAsync()
    {
        var itemsQuery = CreateQuery();

        var items = await ContextHelper.ReadEntitiesAsync(itemsQuery, CancellationToken);
        foreach (var group in items.GroupBy(o => o.OperationType))
        {
            foreach (var chunk in group.Chunk(50))
                await FillItemsAsync(chunk, group.Key);
        }

        return items;
    }

    private IQueryable<UnverifyLogItem> CreateQuery()
    {
        return DbContext.LogItems.AsNoTracking()
            .Where(o => o.CreatedAt <= ExpirationDate && o.ActiveUnverify == null)
            .OrderBy(o => o.CreatedAt);
    }

    private async Task FillItemsAsync(IEnumerable<UnverifyLogItem> items, UnverifyOperationType type)
    {
        switch (type)
        {
            case UnverifyOperationType.Unverify or UnverifyOperationType.SelfUnverify:
                var unverifyQuery = CreateFillQuery<UnverifyLogSetOperation>(items, q => q.Include(o => o.Channels).Include(o => o.Roles));
                var unverifyItems = await ContextHelper.ReadEntitiesAsync(unverifyQuery, CancellationToken);
                foreach (var item in unverifyItems)
                {
                    var logItem = items.FirstOrDefault(o => o.Id == item.LogItemId);
                    if (logItem is not null)
                        logItem.SetOperation = item;
                }
                break;
            case UnverifyOperationType.AutoRemove or UnverifyOperationType.ManualRemove or UnverifyOperationType.Recovery:
                var removeQuery = CreateFillQuery<UnverifyLogRemoveOperation>(items, q => q.Include(o => o.Roles).Include(o => o.Channels));
                var removeItems = await ContextHelper.ReadEntitiesAsync(removeQuery, CancellationToken);
                foreach (var item in removeItems)
                {
                    var logItem = items.FirstOrDefault(o => o.Id == item.LogItemId);
                    if (logItem is not null)
                        logItem.RemoveOperation = item;
                }
                break;
            case UnverifyOperationType.Update:
                var updateQuery = CreateFillQuery<UnverifyLogUpdateOperation>(items);
                var updateItems = await ContextHelper.ReadEntitiesAsync(updateQuery, CancellationToken);
                foreach (var item in updateItems)
                {
                    var logItem = items.FirstOrDefault(o => o.Id == item.LogItemId);
                    if (logItem is not null)
                        logItem.UpdateOperation = item;
                }
                break;
        }
    }

    private IQueryable<TEntity> CreateFillQuery<TEntity>(
        IEnumerable<UnverifyLogItem> items,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null
    ) where TEntity : UnverifyOperationBase
    {
        var logIds = items.Select(o => o.Id).Distinct().ToArray();

        var query = DbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(o => logIds.Contains(o.LogItemId));

        return includes is not null ? includes(query).AsSplitQuery() : query;
    }
}
