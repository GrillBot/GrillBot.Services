using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Archivation;

public partial class CreateArchivationDataAction
{
    private async Task<bool> ExistsItemsToArchiveAsync(DateTime expirationDate)
    {
        int countToArchive;
        using (CreateCounter("Database"))
            countToArchive = await Context.LogItems.AsNoTracking().CountAsync(o => o.CreatedAt <= expirationDate);

        return countToArchive >= AppOptions.MinimalItemsToArchive;
    }

    private async Task<List<LogItem>> ReadItemsToArchiveAsync(DateTime expirationDate)
    {
        var itemsQuery = Context.LogItems.AsNoTracking()
            .Include(o => o.Files)
            .Where(o => o.CreatedAt <= expirationDate)
            .OrderBy(o => o.CreatedAt)
            .Take(AppOptions.MaxItemsToArchivePerRun);

        List<LogItem> items;
        using (CreateCounter("Database"))
            items = await itemsQuery.ToListAsync();

        foreach (var item in items)
        {
            switch (item.Type)
            {
                case LogType.Info or LogType.Warning or LogType.Error:
                    item.LogMessage = await ReadChildDataAsync(Context.LogMessages, item);
                    break;
                case LogType.ChannelCreated:
                    item.ChannelCreated = await ReadChildDataAsync(Context.ChannelCreatedItems.Include(o => o.ChannelInfo), item);
                    break;
                case LogType.ChannelDeleted:
                    item.ChannelDeleted = await ReadChildDataAsync(Context.ChannelDeletedItems.Include(o => o.ChannelInfo), item);
                    break;
                case LogType.ChannelUpdated:
                    item.ChannelUpdated = await ReadChildDataAsync(Context.ChannelUpdatedItems.Include(o => o.Before).Include(o => o.After), item);
                    break;
                case LogType.EmoteDeleted:
                    item.DeletedEmote = await ReadChildDataAsync(Context.DeletedEmotes, item);
                    break;
                case LogType.OverwriteCreated:
                    item.OverwriteCreated = await ReadChildDataAsync(Context.OverwriteCreatedItems.Include(o => o.OverwriteInfo), item);
                    break;
                case LogType.OverwriteDeleted:
                    item.OverwriteDeleted = await ReadChildDataAsync(Context.OverwriteDeletedItems.Include(o => o.OverwriteInfo), item);
                    break;
                case LogType.OverwriteUpdated:
                    item.OverwriteUpdated = await ReadChildDataAsync(Context.OverwriteUpdatedItems.Include(o => o.Before).Include(o => o.After), item);
                    break;
                case LogType.Unban:
                    item.Unban = await ReadChildDataAsync(Context.Unbans, item);
                    break;
                case LogType.MemberUpdated:
                    item.MemberUpdated = await ReadChildDataAsync(Context.MemberUpdatedItems.Include(o => o.Before).Include(o => o.After), item);
                    break;
                case LogType.MemberRoleUpdated:
                    using (CreateCounter("Database"))
                        item.MemberRolesUpdated = (await Context.MemberRoleUpdatedItems.AsNoTracking().Where(o => o.LogItemId == item.Id).ToListAsync()).ToHashSet();
                    break;
                case LogType.GuildUpdated:
                    item.GuildUpdated = await ReadChildDataAsync(Context.GuildUpdatedItems.Include(o => o.Before).Include(o => o.After), item);
                    break;
                case LogType.UserLeft:
                    item.UserLeft = await ReadChildDataAsync(Context.UserLeftItems, item);
                    break;
                case LogType.UserJoined:
                    item.UserJoined = await ReadChildDataAsync(Context.UserJoinedItems, item);
                    break;
                case LogType.MessageEdited:
                    item.MessageEdited = await ReadChildDataAsync(Context.MessageEditedItems, item);
                    break;
                case LogType.MessageDeleted:
                    item.MessageDeleted = await ReadChildDataAsync(Context.MessageDeletedItems.Include(o => o.Embeds).ThenInclude(o => o.Fields), item);
                    break;
                case LogType.InteractionCommand:
                    item.InteractionCommand = await ReadChildDataAsync(Context.InteractionCommands, item);
                    break;
                case LogType.ThreadDeleted:
                    item.ThreadDeleted = await ReadChildDataAsync(Context.ThreadDeletedItems.Include(o => o.ThreadInfo), item);
                    break;
                case LogType.JobCompleted:
                    item.Job = await ReadChildDataAsync(Context.JobExecutions, item);
                    break;
                case LogType.Api:
                    item.ApiRequest = await ReadChildDataAsync(Context.ApiRequests, item);
                    break;
                case LogType.ThreadUpdated:
                    item.ThreadUpdated = await ReadChildDataAsync(Context.ThreadUpdatedItems.Include(o => o.Before).Include(o => o.After), item);
                    break;
                case LogType.RoleDeleted:
                    item.RoleDeleted = await ReadChildDataAsync(Context.RoleDeleted.Include(o => o.RoleInfo), item);
                    break;
            }
        }

        return items;
    }

    private async Task<TData?> ReadChildDataAsync<TData>(IQueryable<TData> query, LogItem header) where TData : ChildEntityBase
    {
        query = query.Where(o => o.LogItemId == header.Id).AsNoTracking();

        using (CreateCounter("Database"))
            return await query.FirstOrDefaultAsync();
    }
}
