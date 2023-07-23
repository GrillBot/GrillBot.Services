using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices;

public partial class PostProcessingService
{
    private async Task<LogItem?> ReadLogItemToProcessAsync(CancellationToken cancellationToken)
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditLogServiceContext>();

        var metadata = await context.LogItems.AsNoTracking()
            .Where(o => (o.Flags & LogItemFlag.ToProcess) != 0)
            .OrderBy(o => o.CreatedAt)
            .Select(o => new { o.Id, o.Type })
            .FirstOrDefaultAsync(cancellationToken);
        if (metadata is null)
            return null;

        return await ReadLogItemAsync(context, metadata.Id, metadata.Type, true, cancellationToken);
    }

    private static async Task<LogItem?> ReadLogItemAsync(AuditLogServiceContext context, Guid id, LogType type, bool disableTracking, CancellationToken cancellationToken)
    {
        var query = context.LogItems.Include(o => o.Files).Where(o => o.Id == id);
        switch (type)
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
            case LogType.InteractionCommand:
                query = query.Include(o => o.InteractionCommand);
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

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    private async Task FinishLogItemProcessingAsync(LogItem oldLogItem, CancellationToken cancellationToken)
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditLogServiceContext>();

        var latestLogItem = await ReadLogItemAsync(context, oldLogItem.Id, oldLogItem.Type, false, cancellationToken);
        if (latestLogItem is null)
            return;

        if (latestLogItem.IsDeleted)
        {
            if (!oldLogItem.IsDeleted)
                latestLogItem.Flags = LogItemFlag.ToProcess | LogItemFlag.Deleted;
            else
                context.Remove(latestLogItem);
        }
        else
        {
            latestLogItem.Flags = LogItemFlag.None;
        }

        await context.SaveChangesAsync();
    }
}
