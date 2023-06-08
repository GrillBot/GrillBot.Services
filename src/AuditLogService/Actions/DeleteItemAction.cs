using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Response;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions;

public class DeleteItemAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public DeleteItemAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var id = (Guid)Parameters[0]!;
        var response = new DeleteItemResponse();

        var logItem = await Context.LogItems.FirstOrDefaultAsync(o => o.Id == id);
        if (logItem is null)
            return new ApiResult(StatusCodes.Status404NotFound, response);

        Context.Remove(logItem);
        await SetFilesAndRemoveAsync(response, logItem);
        await DeleteChildItemsAsync(id, logItem.Type);
        await Context.SaveChangesAsync();

        response.Exists = true;
        return new ApiResult(StatusCodes.Status200OK, response);
    }

    private async Task SetFilesAndRemoveAsync(DeleteItemResponse response, LogItem item)
    {
        var files = await Context.Files.Where(o => o.LogItemId == item.Id).ToListAsync();
        if (files.Count == 0)
            return;

        response.FilesToDelete.AddRange(files.Select(o => o.Filename));
        Context.RemoveRange(files);
    }

    private async Task DeleteChildItemsAsync(Guid logItemId, LogType type)
    {
        switch (type)
        {
            case LogType.Info or LogType.Warning or LogType.Error:
                Context.RemoveRange(await Context.LogMessages.Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.ChannelCreated:
                Context.RemoveRange(await Context.ChannelCreatedItems.Include(o => o.ChannelInfo).Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.ChannelDeleted:
                Context.RemoveRange(await Context.ChannelDeletedItems.Include(o => o.ChannelInfo).Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.ChannelUpdated:
                Context.RemoveRange(await Context.ChannelUpdatedItems.Include(o => o.Before).Include(o => o.After).Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.EmoteDeleted:
                Context.RemoveRange(await Context.DeletedEmotes.Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.OverwriteCreated:
                Context.RemoveRange(await Context.OverwriteCreatedItems.Include(o => o.OverwriteInfo).Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.OverwriteDeleted:
                Context.RemoveRange(await Context.OverwriteDeletedItems.Include(o => o.OverwriteInfo).Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.OverwriteUpdated:
                Context.RemoveRange(await Context.OverwriteUpdatedItems.Include(o => o.Before).Include(o => o.After).Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.Unban:
                Context.RemoveRange(await Context.Unbans.Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.MemberUpdated:
                Context.RemoveRange(await Context.MemberUpdatedItems.Include(o => o.Before).Include(o => o.After).Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.MemberRoleUpdated:
                Context.RemoveRange(await Context.MemberRoleUpdatedItems.Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.GuildUpdated:
                Context.RemoveRange(await Context.GuildUpdatedItems.Include(o => o.Before).Include(o => o.After).Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.UserLeft:
                Context.RemoveRange(await Context.UserLeftItems.Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.UserJoined:
                Context.RemoveRange(await Context.UserJoinedItems.Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.MessageEdited:
                Context.RemoveRange(await Context.MessageEditedItems.Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.MessageDeleted:
                Context.RemoveRange(await Context.MessageDeletedItems.Include(o => o.Embeds).ThenInclude(o => o.Fields).Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.ThreadDeleted:
                Context.RemoveRange(await Context.ThreadDeletedItems.Include(o => o.ThreadInfo).Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.JobCompleted:
                Context.RemoveRange(await Context.JobExecutions.Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.Api:
                Context.RemoveRange(await Context.ApiRequests.Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
            case LogType.ThreadUpdated:
                Context.RemoveRange(await Context.ThreadUpdatedItems.Include(o => o.Before).Include(o => o.After).Where(o => o.LogItemId == logItemId).ToListAsync());
                break;
        }
    }
}
