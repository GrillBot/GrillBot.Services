using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Response;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace AuditLogService.Actions;

public class DeleteItemAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }
    private Channel<LogItem> Channel { get; }

    public DeleteItemAction(AuditLogServiceContext context, Channel<LogItem> channel)
    {
        Context = context;
        Channel = channel;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var id = (Guid)Parameters[0]!;
        var response = new DeleteItemResponse();

        var logItem = await ReadLogItemAsync(id);
        if (logItem is null)
            return new ApiResult(StatusCodes.Status404NotFound, response);

        response.FilesToDelete = logItem.Files.Select(o => o.Filename).Distinct().ToList();
        response.Exists = true;

        Context.Remove(logItem);
        await Context.SaveChangesAsync();
        await Channel.Writer.WriteAsync(logItem);

        return ApiResult.FromSuccess(response);
    }

    private async Task<LogItem?> ReadLogItemAsync(Guid id)
    {
        var logItemType = await Context.LogItems.AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => o.Type)
            .FirstOrDefaultAsync();
        if (logItemType is LogType.None)
            return null;

        var query = Context.LogItems.Include(o => o.Files).Where(o => o.Id == id);
        switch (logItemType)
        {
            case LogType.Info or LogType.Warning or LogType.Error:
                query = query.Include(o => o.LogMessage);
                break;
            case LogType.ChannelCreated:
                query = query.Include(o => o.ChannelCreated!.ChannelInfo);
                break;
            case LogType.ChannelDeleted:
                query = query.Include(o => o.ChannelDeleted!.ChannelInfo);
                break;
            case LogType.ChannelUpdated:
                query = query.Include(o => o.ChannelUpdated!.Before);
                query = query.Include(o => o.ChannelUpdated!.After);
                break;
            case LogType.EmoteDeleted:
                query = query.Include(o => o.DeletedEmote);
                break;
            case LogType.OverwriteCreated:
                query = query.Include(o => o.OverwriteCreated!.OverwriteInfo);
                break;
            case LogType.OverwriteDeleted:
                query = query.Include(o => o.OverwriteDeleted!.OverwriteInfo);
                break;
            case LogType.OverwriteUpdated:
                query = query.Include(o => o.OverwriteUpdated!.Before);
                query = query.Include(o => o.OverwriteUpdated!.After);
                break;
            case LogType.Unban:
                query = query.Include(o => o.Unban);
                break;
            case LogType.MemberUpdated:
                query = query.Include(o => o.MemberUpdated!.Before);
                query = query.Include(o => o.MemberUpdated!.After);
                break;
            case LogType.MemberRoleUpdated:
                query = query.Include(o => o.MemberRolesUpdated);
                break;
            case LogType.GuildUpdated:
                query = query.Include(o => o.GuildUpdated!.Before);
                query = query.Include(o => o.GuildUpdated!.After);
                break;
            case LogType.UserLeft:
                query = query.Include(o => o.UserLeft);
                break;
            case LogType.UserJoined:
                query = query.Include(o => o.UserJoined);
                break;
            case LogType.MessageEdited:
                query = query.Include(o => o.MessageEdited);
                break;
            case LogType.MessageDeleted:
                query = query.Include(o => o.MessageDeleted!.Embeds).ThenInclude(o => o.Fields);
                break;
            case LogType.ThreadDeleted:
                query = query.Include(o => o.ThreadDeleted!.ThreadInfo);
                break;
            case LogType.JobCompleted:
                query = query.Include(o => o.Job);
                break;
            case LogType.Api:
                query = query.Include(o => o.ApiRequest);
                break;
            case LogType.ThreadUpdated:
                query = query.Include(o => o.ThreadUpdated!.Before);
                query = query.Include(o => o.ThreadUpdated!.After);
                break;
        }

        return await query.FirstOrDefaultAsync();
    }
}
