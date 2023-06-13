using AuditLogService.Core.Entity;

namespace AuditLogService.Actions.Archivation;

public partial class ArchiveOldLogsAction
{
    private void RemoveItem(LogItem item)
    {
        if (item.Files.Count > 0)
            Context.RemoveRange(item.Files);

        RemoveItem(item.ApiRequest);
        RemoveItem(item.LogMessage);
        RemoveItem(item.DeletedEmote);
        RemoveItem(item.Unban);
        RemoveItem(item.Job);
        RemoveItem(item.ChannelCreated);
        RemoveItem(item.ChannelDeleted);
        RemoveItem(item.ChannelUpdated);
        RemoveItem(item.GuildUpdated);
        RemoveItem(item.MemberRolesUpdated);
        RemoveItem(item.MessageDeleted);
        RemoveItem(item.MessageEdited);
        RemoveItem(item.OverwriteCreated);
        RemoveItem(item.OverwriteUpdated);
        RemoveItem(item.OverwriteDeleted);
        RemoveItem(item.UserJoined);
        RemoveItem(item.UserLeft);
        RemoveItem(item.InteractionCommand);
        RemoveItem(item.ThreadDeleted);
        RemoveItem(item.ThreadUpdated);
        RemoveItem(item.MemberUpdated);
        Context.Remove(item);
    }

    private void RemoveItem<TEntity>(TEntity? entity) where TEntity : class
    {
        if (entity is not null)
            Context.Set<TEntity>().Remove(entity);
    }
}
