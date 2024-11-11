using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Archivation;

public partial class CreateArchivationDataAction
{
    private async Task<bool> ExistsItemsToArchiveAsync(DateTime expirationDate)
    {
        var query = DbContext.LogItems.Where(o => o.CreatedAt <= expirationDate || o.IsDeleted);
        var countToArchive = await ContextHelper.ReadCountAsync(query);

        return countToArchive >= AppOptions.MinimalItemsToArchive;
    }

    private async Task<List<LogItem>> ReadItemsToArchiveAsync(DateTime expirationDate)
    {
        var itemsQuery = DbContext.LogItems.AsNoTracking()
            .Include(o => o.Files)
            .Where(o => o.CreatedAt <= expirationDate || o.IsDeleted)
            .OrderBy(o => o.CreatedAt)
            .Take(AppOptions.MaxItemsToArchivePerRun);

        var items = await ContextHelper.ReadEntitiesAsync(itemsQuery);
        foreach (var item in items.GroupBy(o => o.Type))
        {
            foreach (var chunk in item.Chunk(50))
                await FillItemsAsync(chunk, item.Key);
        }

        return items;
    }

    private async Task FillItemsAsync(IEnumerable<LogItem> headers, LogType type)
    {
        switch (type)
        {
            case LogType.Info or LogType.Warning or LogType.Error:
                await SetLogDataAsync(headers, DbContext.LogMessages, (header, item) => header.LogMessage = item);
                break;
            case LogType.ChannelCreated:
                await SetLogDataAsync(headers, DbContext.ChannelCreatedItems.Include(o => o.ChannelInfo), (header, item) => header.ChannelCreated = item);
                break;
            case LogType.ChannelDeleted:
                await SetLogDataAsync(headers, DbContext.ChannelDeletedItems.Include(o => o.ChannelInfo), (header, item) => header.ChannelDeleted = item);
                break;
            case LogType.ChannelUpdated:
                await SetLogDataAsync(headers, DbContext.ChannelUpdatedItems.Include(o => o.Before).Include(o => o.After), (header, item) => header.ChannelUpdated = item);
                break;
            case LogType.EmoteDeleted:
                await SetLogDataAsync(headers, DbContext.DeletedEmotes, (header, item) => header.DeletedEmote = item);
                break;
            case LogType.OverwriteCreated:
                await SetLogDataAsync(headers, DbContext.OverwriteCreatedItems.Include(o => o.OverwriteInfo), (header, item) => header.OverwriteCreated = item);
                break;
            case LogType.OverwriteDeleted:
                await SetLogDataAsync(headers, DbContext.OverwriteDeletedItems.Include(o => o.OverwriteInfo), (header, item) => header.OverwriteDeleted = item);
                break;
            case LogType.OverwriteUpdated:
                await SetLogDataAsync(headers, DbContext.OverwriteUpdatedItems.Include(o => o.Before).Include(o => o.After), (header, item) => header.OverwriteUpdated = item);
                break;
            case LogType.Unban:
                await SetLogDataAsync(headers, DbContext.Unbans, (header, item) => header.Unban = item);
                break;
            case LogType.MemberUpdated:
                await SetLogDataAsync(headers, DbContext.MemberUpdatedItems.Include(o => o.Before).Include(o => o.After), (header, item) => header.MemberUpdated = item);
                break;
            case LogType.MemberRoleUpdated:
                await SetLogDataWithoutKeyAsync(headers, DbContext.MemberRoleUpdatedItems, (header, item) =>
                {
                    header.MemberRolesUpdated ??= new HashSet<MemberRoleUpdated>();
                    header.MemberRolesUpdated.Add(item);
                });
                break;
            case LogType.GuildUpdated:
                await SetLogDataAsync(headers, DbContext.GuildUpdatedItems.Include(o => o.Before).Include(o => o.After), (header, item) => header.GuildUpdated = item);
                break;
            case LogType.UserLeft:
                await SetLogDataAsync(headers, DbContext.UserLeftItems, (header, item) => header.UserLeft = item);
                break;
            case LogType.UserJoined:
                await SetLogDataAsync(headers, DbContext.UserJoinedItems, (header, item) => header.UserJoined = item);
                break;
            case LogType.MessageEdited:
                await SetLogDataAsync(headers, DbContext.MessageEditedItems, (header, item) => header.MessageEdited = item);
                break;
            case LogType.MessageDeleted:
                await SetLogDataAsync(headers, DbContext.MessageDeletedItems, (header, item) => header.MessageDeleted = item);
                break;
            case LogType.ThreadDeleted:
                await SetLogDataAsync(headers, DbContext.ThreadDeletedItems.Include(o => o.ThreadInfo), (header, item) => header.ThreadDeleted = item);
                break;
            case LogType.ThreadUpdated:
                await SetLogDataAsync(headers, DbContext.ThreadUpdatedItems.Include(o => o.Before).Include(o => o.After), (header, item) => header.ThreadUpdated = item);
                break;
            case LogType.JobCompleted:
                await SetLogDataAsync(headers, DbContext.JobExecutions, (header, item) => header.Job = item);
                break;
            case LogType.Api:
                await SetLogDataAsync(headers, DbContext.ApiRequests, (header, item) => header.ApiRequest = item);
                break;
            case LogType.RoleDeleted:
                await SetLogDataAsync(headers, DbContext.RoleDeleted.Include(o => o.RoleInfo), (header, item) => header.RoleDeleted = item);
                break;
        }
    }

    private async Task SetLogDataAsync<TData>(IEnumerable<LogItem> headers, IQueryable<TData> query, Action<LogItem, TData> setData) where TData : ChildEntityBase
        => await headers.SetLogDataAsync(query, setData, ContextHelper, true);

    private async Task SetLogDataWithoutKeyAsync<TData>(IEnumerable<LogItem> headers, IQueryable<TData> query, Action<LogItem, TData> setData) where TData : ChildEntityBaseWithoutKey
        => await headers.SetLogDataWithoutKeyAsync(query, setData, ContextHelper, true);
}
