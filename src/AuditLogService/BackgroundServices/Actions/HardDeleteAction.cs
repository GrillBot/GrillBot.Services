using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class HardDeleteAction : PostProcessActionBase
{
    public HardDeleteAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => true;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var itemsToDelete = await Context.LogItems
            .Where(o => o.IsDeleted && !o.IsPendingProcess && o.Type == logItem.Type)
            .ToListAsync();

        foreach (var item in itemsToDelete)
        {
            Context.Remove(item);

            switch (item.Type)
            {
                case LogType.Info or LogType.Warning or LogType.Error:
                    await RemoveChildDataAsync<LogMessage>(item);
                    break;
                case LogType.ChannelCreated:
                    {
                        var childItems = await RemoveChildDataAsync<ChannelCreated>(item, q => q.Include(o => o.ChannelInfo));
                        Context.RemoveRange(childItems.Where(o => o.ChannelInfo is not null).Select(o => o.ChannelInfo));
                    }
                    break;
                case LogType.ChannelDeleted:
                    {
                        var childItems = await RemoveChildDataAsync<ChannelDeleted>(item, q => q.Include(o => o.ChannelInfo));
                        Context.RemoveRange(childItems.Where(o => o.ChannelInfo is not null).Select(o => o.ChannelInfo));
                    }
                    break;
                case LogType.ChannelUpdated:
                    {
                        var childItems = await RemoveChildDataAsync<ChannelUpdated>(item, q => q.Include(o => o.After).Include(o => o.Before));
                        Context.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        Context.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.EmoteDeleted:
                    await RemoveChildDataAsync<DeletedEmote>(item);
                    break;
                case LogType.OverwriteCreated:
                    {
                        var childItems = await RemoveChildDataAsync<OverwriteCreated>(item, q => q.Include(o => o.OverwriteInfo));
                        Context.RemoveRange(childItems.Where(o => o.OverwriteInfo is not null).Select(o => o.OverwriteInfo));
                    }
                    break;
                case LogType.OverwriteDeleted:
                    {
                        var childItems = await RemoveChildDataAsync<OverwriteDeleted>(item, q => q.Include(o => o.OverwriteInfo));
                        Context.RemoveRange(childItems.Where(o => o.OverwriteInfo is not null).Select(o => o.OverwriteInfo));
                    }
                    break;
                case LogType.OverwriteUpdated:
                    {
                        var childItems = await RemoveChildDataAsync<OverwriteUpdated>(item, q => q.Include(o => o.After).Include(o => o.Before));
                        Context.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        Context.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.Unban:
                    await RemoveChildDataAsync<Unban>(item);
                    break;
                case LogType.MemberUpdated:
                    {
                        var childItems = await RemoveChildDataAsync<MemberUpdated>(item, q => q.Include(o => o.After).Include(o => o.Before));
                        Context.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        Context.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.GuildUpdated:
                    {
                        var childItems = await RemoveChildDataAsync<GuildUpdated>(item, q => q.Include(o => o.After).Include(o => o.Before));
                        Context.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        Context.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.UserLeft:
                    await RemoveChildDataAsync<UserLeft>(item);
                    break;
                case LogType.UserJoined:
                    await RemoveChildDataAsync<UserJoined>(item);
                    break;
                case LogType.MessageEdited:
                    await RemoveChildDataAsync<MessageEdited>(item);
                    break;
                case LogType.MessageDeleted:
                    {
                        var childItems = await RemoveChildDataAsync<MessageDeleted>(item, q => q.Include(o => o.Embeds).ThenInclude(o => o.Fields));
                        foreach (var childItem in childItems)
                        {
                            Context.Remove(childItem);
                            foreach (var embed in childItem.Embeds)
                            {
                                Context.Remove(embed);
                                foreach (var field in embed.Fields)
                                    Context.Remove(field);
                            }
                        }
                    }
                    break;
                case LogType.InteractionCommand:
                    await RemoveChildDataAsync<InteractionCommand>(item);
                    break;
                case LogType.ThreadDeleted:
                    {
                        var childItems = await RemoveChildDataAsync<ThreadDeleted>(item, q => q.Include(o => o.ThreadInfo));
                        Context.RemoveRange(childItems.Where(o => o.ThreadInfo is not null).Select(o => o.ThreadInfo));
                    }
                    break;
                case LogType.JobCompleted:
                    await RemoveChildDataAsync<JobExecution>(item);
                    break;
                case LogType.Api:
                    await RemoveChildDataAsync<ApiRequest>(item);
                    break;
                case LogType.ThreadUpdated:
                    {
                        var childItems = await RemoveChildDataAsync<ThreadUpdated>(item, q => q.Include(o => o.After).Include(o => o.Before));
                        Context.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        Context.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
            }
        }

        await Context.SaveChangesAsync();
    }

    private async Task<List<TChildType>> RemoveChildDataAsync<TChildType>(LogItem parent, Func<IQueryable<TChildType>, IQueryable<TChildType>>? includeAction = null) where TChildType : ChildEntityBase
    {
        var query = Context.Set<TChildType>().Where(o => o.LogItemId == parent.Id);
        if (includeAction is not null)
            query = includeAction(query);

        var childItem = await query.ToListAsync();
        Context.RemoveRange(childItem);

        return childItem;
    }
}
