using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Archivation;

public partial class CreateArchivationDataAction
{
    private async Task<bool> ExistsItemsToArchiveAsync(DateTime expirationDate)
    {
        var query = DbContext.LogItems.Where(o => o.CreatedAt <= expirationDate);
        var countToArchive = await ContextHelper.ReadCountAsync(query);

        return countToArchive >= AppOptions.MinimalItemsToArchive;
    }

    private async Task<List<LogItem>> ReadItemsToArchiveAsync(DateTime expirationDate)
    {
        var itemsQuery = DbContext.LogItems.AsNoTracking()
            .Include(o => o.Files)
            .Where(o => o.CreatedAt <= expirationDate)
            .OrderBy(o => o.CreatedAt)
            .Take(AppOptions.MaxItemsToArchivePerRun);

        var items = await ContextHelper.ReadEntitiesAsync(itemsQuery);
        foreach (var item in items)
        {
            switch (item.Type)
            {
                case LogType.Info or LogType.Warning or LogType.Error:
                    item.LogMessage = await ReadChildDataAsync(DbContext.LogMessages, item);
                    break;
                case LogType.ChannelCreated:
                    item.ChannelCreated = await ReadChildDataAsync(DbContext.ChannelCreatedItems.Include(o => o.ChannelInfo), item);
                    break;
                case LogType.ChannelDeleted:
                    item.ChannelDeleted = await ReadChildDataAsync(DbContext.ChannelDeletedItems.Include(o => o.ChannelInfo), item);
                    break;
                case LogType.ChannelUpdated:
                    item.ChannelUpdated = await ReadChildDataAsync(DbContext.ChannelUpdatedItems.Include(o => o.Before).Include(o => o.After), item);
                    break;
                case LogType.EmoteDeleted:
                    item.DeletedEmote = await ReadChildDataAsync(DbContext.DeletedEmotes, item);
                    break;
                case LogType.OverwriteCreated:
                    item.OverwriteCreated = await ReadChildDataAsync(DbContext.OverwriteCreatedItems.Include(o => o.OverwriteInfo), item);
                    break;
                case LogType.OverwriteDeleted:
                    item.OverwriteDeleted = await ReadChildDataAsync(DbContext.OverwriteDeletedItems.Include(o => o.OverwriteInfo), item);
                    break;
                case LogType.OverwriteUpdated:
                    item.OverwriteUpdated = await ReadChildDataAsync(DbContext.OverwriteUpdatedItems.Include(o => o.Before).Include(o => o.After), item);
                    break;
                case LogType.Unban:
                    item.Unban = await ReadChildDataAsync(DbContext.Unbans, item);
                    break;
                case LogType.MemberUpdated:
                    item.MemberUpdated = await ReadChildDataAsync(DbContext.MemberUpdatedItems.Include(o => o.Before).Include(o => o.After), item);
                    break;
                case LogType.MemberRoleUpdated:
                    using (CreateCounter("Database"))
                        item.MemberRolesUpdated = (await DbContext.MemberRoleUpdatedItems.AsNoTracking().Where(o => o.LogItemId == item.Id).ToListAsync()).ToHashSet();
                    break;
                case LogType.GuildUpdated:
                    item.GuildUpdated = await ReadChildDataAsync(DbContext.GuildUpdatedItems.Include(o => o.Before).Include(o => o.After), item);
                    break;
                case LogType.UserLeft:
                    item.UserLeft = await ReadChildDataAsync(DbContext.UserLeftItems, item);
                    break;
                case LogType.UserJoined:
                    item.UserJoined = await ReadChildDataAsync(DbContext.UserJoinedItems, item);
                    break;
                case LogType.MessageEdited:
                    item.MessageEdited = await ReadChildDataAsync(DbContext.MessageEditedItems, item);
                    break;
                case LogType.MessageDeleted:
                    item.MessageDeleted = await ReadChildDataAsync(DbContext.MessageDeletedItems.Include(o => o.Embeds).ThenInclude(o => o.Fields), item);
                    break;
                case LogType.InteractionCommand:
                    item.InteractionCommand = await ReadChildDataAsync(DbContext.InteractionCommands, item);
                    break;
                case LogType.ThreadDeleted:
                    item.ThreadDeleted = await ReadChildDataAsync(DbContext.ThreadDeletedItems.Include(o => o.ThreadInfo), item);
                    break;
                case LogType.JobCompleted:
                    item.Job = await ReadChildDataAsync(DbContext.JobExecutions, item);
                    break;
                case LogType.Api:
                    item.ApiRequest = await ReadChildDataAsync(DbContext.ApiRequests, item);
                    break;
                case LogType.ThreadUpdated:
                    item.ThreadUpdated = await ReadChildDataAsync(DbContext.ThreadUpdatedItems.Include(o => o.Before).Include(o => o.After), item);
                    break;
                case LogType.RoleDeleted:
                    item.RoleDeleted = await ReadChildDataAsync(DbContext.RoleDeleted.Include(o => o.RoleInfo), item);
                    break;
            }
        }

        return items;
    }

    private async Task<TData?> ReadChildDataAsync<TData>(IQueryable<TData> query, LogItem header) where TData : ChildEntityBase
    {
        query = query.Where(o => o.LogItemId == header.Id).AsNoTracking();
        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
    }
}
