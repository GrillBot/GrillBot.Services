using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Archivation;

public partial class ArchiveOldLogsAction
{
    private async Task<bool> ExistsItemsToArchiveAsync(DateTime expirationDate)
    {
        var countToArchive = await Context.LogItems.AsNoTracking().CountAsync(o => o.CreatedAt <= expirationDate);
        return countToArchive >= AppOptions.ItemsToArchive;
    }

    private async Task<List<LogItem>> ReadItemsToArchiveAsync(DateTime expirationDate)
    {
        var items = await Context.LogItems
            .Include(o => o.Files)
            .Where(o => o.CreatedAt <= expirationDate)
            .ToListAsync();

        foreach (var item in items)
        {
            switch (item.Type)
            {
                case LogType.Info or LogType.Warning or LogType.Error:
                    item.LogMessage = await Context.LogMessages.FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.ChannelCreated:
                    item.ChannelCreated = await Context.ChannelCreatedItems.Include(o => o.ChannelInfo).FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.ChannelDeleted:
                    item.ChannelDeleted = await Context.ChannelDeletedItems.Include(o => o.ChannelInfo).FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.ChannelUpdated:
                    item.ChannelUpdated = await Context.ChannelUpdatedItems.Include(o => o.Before).Include(o => o.After).FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.EmoteDeleted:
                    item.DeletedEmote = await Context.DeletedEmotes.FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.OverwriteCreated:
                    item.OverwriteCreated = await Context.OverwriteCreatedItems.Include(o => o.OverwriteInfo).FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.OverwriteDeleted:
                    item.OverwriteDeleted = await Context.OverwriteDeletedItems.Include(o => o.OverwriteInfo).FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.OverwriteUpdated:
                    item.OverwriteUpdated = await Context.OverwriteUpdatedItems.Include(o => o.Before).Include(o => o.After).FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.Unban:
                    item.Unban = await Context.Unbans.FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.MemberUpdated:
                    item.MemberUpdated = await Context.MemberUpdatedItems.Include(o => o.Before).Include(o => o.After).FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.MemberRoleUpdated:
                    item.MemberRolesUpdated = (await Context.MemberRoleUpdatedItems.Where(o => o.LogItemId == item.Id).ToListAsync()).ToHashSet();
                    break;
                case LogType.GuildUpdated:
                    item.GuildUpdated = await Context.GuildUpdatedItems.Include(o => o.Before).Include(o => o.After).FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.UserLeft:
                    item.UserLeft = await Context.UserLeftItems.FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.UserJoined:
                    item.UserJoined = await Context.UserJoinedItems.FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.MessageEdited:
                    item.MessageEdited = await Context.MessageEditedItems.FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.MessageDeleted:
                    item.MessageDeleted = await Context.MessageDeletedItems.Include(o => o.Embeds).ThenInclude(o => o.Fields).FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.InteractionCommand:
                    item.InteractionCommand = await Context.InteractionCommands.FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.ThreadDeleted:
                    item.ThreadDeleted = await Context.ThreadDeletedItems.Include(o => o.ThreadInfo).FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.JobCompleted:
                    item.Job = await Context.JobExecutions.FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.Api:
                    item.ApiRequest = await Context.ApiRequests.FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
                case LogType.ThreadUpdated:
                    item.ThreadUpdated = await Context.ThreadUpdatedItems.Include(o => o.Before).Include(o => o.After).FirstOrDefaultAsync(o => o.LogItemId == item.Id);
                    break;
            }
        }

        return items;
    }
}
