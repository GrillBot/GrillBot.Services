using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Archivation;

public partial class CreateArchivationDataAction
{
    private async Task<bool> ExistsItemsToArchiveAsync(DateTime expirationDate)
    {
        var countToArchive = await Context.LogItems.AsNoTracking()
            .CountAsync(o => !o.IsDeleted && o.CreatedAt <= expirationDate);

        return countToArchive >= AppOptions.MinimalItemsToArchive;
    }

    private async Task<List<LogItem>> ReadItemsToArchiveAsync(DateTime expirationDate)
    {
        IQueryable<TData> WithCommonFilter<TData>(IQueryable<TData> query, LogItem item) where TData : ChildEntityBase
            => query.Where(o => o.LogItemId == item.Id).AsNoTracking();

        var items = await Context.LogItems.AsNoTracking()
            .Include(o => o.Files)
            .Where(o => !o.IsDeleted && o.CreatedAt <= expirationDate)
            .OrderBy(o => o.CreatedAt)
            .Take(AppOptions.MaxItemsToArchivePerRun)
            .ToListAsync();

        foreach (var item in items)
        {
            switch (item.Type)
            {
                case LogType.Info or LogType.Warning or LogType.Error:
                    item.LogMessage = await WithCommonFilter(Context.LogMessages, item).FirstOrDefaultAsync();
                    break;
                case LogType.ChannelCreated:
                    item.ChannelCreated = await WithCommonFilter(Context.ChannelCreatedItems, item).Include(o => o.ChannelInfo).FirstOrDefaultAsync();
                    break;
                case LogType.ChannelDeleted:
                    item.ChannelDeleted = await WithCommonFilter(Context.ChannelDeletedItems, item).Include(o => o.ChannelInfo).FirstOrDefaultAsync();
                    break;
                case LogType.ChannelUpdated:
                    item.ChannelUpdated = await WithCommonFilter(Context.ChannelUpdatedItems, item).Include(o => o.Before).Include(o => o.After).FirstOrDefaultAsync();
                    break;
                case LogType.EmoteDeleted:
                    item.DeletedEmote = await WithCommonFilter(Context.DeletedEmotes, item).FirstOrDefaultAsync();
                    break;
                case LogType.OverwriteCreated:
                    item.OverwriteCreated = await WithCommonFilter(Context.OverwriteCreatedItems, item).Include(o => o.OverwriteInfo).FirstOrDefaultAsync();
                    break;
                case LogType.OverwriteDeleted:
                    item.OverwriteDeleted = await WithCommonFilter(Context.OverwriteDeletedItems, item).Include(o => o.OverwriteInfo).FirstOrDefaultAsync();
                    break;
                case LogType.OverwriteUpdated:
                    item.OverwriteUpdated = await WithCommonFilter(Context.OverwriteUpdatedItems, item).Include(o => o.Before).Include(o => o.After).FirstOrDefaultAsync();
                    break;
                case LogType.Unban:
                    item.Unban = await WithCommonFilter(Context.Unbans, item).FirstOrDefaultAsync();
                    break;
                case LogType.MemberUpdated:
                    item.MemberUpdated = await WithCommonFilter(Context.MemberUpdatedItems, item).Include(o => o.Before).Include(o => o.After).FirstOrDefaultAsync();
                    break;
                case LogType.MemberRoleUpdated:
                    item.MemberRolesUpdated = (await Context.MemberRoleUpdatedItems.AsNoTracking().Where(o => o.LogItemId == item.Id).ToListAsync()).ToHashSet();
                    break;
                case LogType.GuildUpdated:
                    item.GuildUpdated = await WithCommonFilter(Context.GuildUpdatedItems, item).Include(o => o.Before).Include(o => o.After).FirstOrDefaultAsync();
                    break;
                case LogType.UserLeft:
                    item.UserLeft = await WithCommonFilter(Context.UserLeftItems, item).FirstOrDefaultAsync();
                    break;
                case LogType.UserJoined:
                    item.UserJoined = await WithCommonFilter(Context.UserJoinedItems, item).FirstOrDefaultAsync();
                    break;
                case LogType.MessageEdited:
                    item.MessageEdited = await WithCommonFilter(Context.MessageEditedItems, item).FirstOrDefaultAsync();
                    break;
                case LogType.MessageDeleted:
                    item.MessageDeleted = await WithCommonFilter(Context.MessageDeletedItems, item).Include(o => o.Embeds).ThenInclude(o => o.Fields).FirstOrDefaultAsync();
                    break;
                case LogType.InteractionCommand:
                    item.InteractionCommand = await WithCommonFilter(Context.InteractionCommands, item).FirstOrDefaultAsync();
                    break;
                case LogType.ThreadDeleted:
                    item.ThreadDeleted = await WithCommonFilter(Context.ThreadDeletedItems, item).Include(o => o.ThreadInfo).FirstOrDefaultAsync();
                    break;
                case LogType.JobCompleted:
                    item.Job = await WithCommonFilter(Context.JobExecutions, item).FirstOrDefaultAsync();
                    break;
                case LogType.Api:
                    item.ApiRequest = await WithCommonFilter(Context.ApiRequests, item).FirstOrDefaultAsync();
                    break;
                case LogType.ThreadUpdated:
                    item.ThreadUpdated = await WithCommonFilter(Context.ThreadUpdatedItems, item).Include(o => o.Before).Include(o => o.After).FirstOrDefaultAsync();
                    break;
                case LogType.RoleDeleted:
                    item.RoleDeleted = await WithCommonFilter(Context.RoleDeleted, item).Include(o => o.RoleInfo).FirstOrDefaultAsync();
                    break;
            }
        }

        return items;
    }
}
