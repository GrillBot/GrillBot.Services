using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Managers;
using AuditLogService.Models.Events;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers;

public class BulkDeleteEventHandler : BaseEventHandlerWithDb<BulkDeletePayload, AuditLogServiceContext>
{
    private DataRecalculationManager DataRecalculation { get; }

    public BulkDeleteEventHandler(ILoggerFactory loggerFactory, AuditLogServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher, DataRecalculationManager dataRecalculation) : base(loggerFactory, dbContext, counterManager, publisher)
    {
        DataRecalculation = dataRecalculation;
    }

    protected override async Task HandleInternalAsync(BulkDeletePayload payload)
    {
        var logItems = await ReadLogItemsAsync(payload.Ids);

        foreach (var logItem in logItems)
        {
            DbContext.Remove(logItem);

            await MarkDeletionOfChildDataAsync(logItem);
            await EnqueueFileDeletionAsync(logItem);
        }

        await ContextHelper.SaveChagesAsync();
        await DataRecalculation.EnqueueRecalculationAsync(logItems);
    }

    private async Task<List<LogItem>> ReadLogItemsAsync(List<Guid> ids)
    {
        var result = new List<LogItem>();
        var baseQuery = DbContext.LogItems.Include(o => o.Files);

        foreach (var chunk in ids.Distinct().Chunk(100))
        {
            var query = baseQuery.Where(o => chunk.Contains(o.Id));
            result.AddRange(await ContextHelper.ReadEntitiesAsync(query));
        }

        return result;
    }

    private async Task MarkDeletionOfChildDataAsync(LogItem item)
    {
        switch (item.Type)
        {
            case LogType.Info or LogType.Warning or LogType.Error:
                await RemoveChildDataAsync<LogMessage>(item);
                break;
            case LogType.ChannelCreated:
                {
                    var childItems = await RemoveChildDataAsync<ChannelCreated>(item, q => q.Include(o => o.ChannelInfo));
                    DbContext.RemoveRange(childItems.Where(o => o.ChannelInfo is not null).Select(o => o.ChannelInfo));
                }
                break;
            case LogType.ChannelDeleted:
                {
                    var childItems = await RemoveChildDataAsync<ChannelDeleted>(item, q => q.Include(o => o.ChannelInfo));
                    DbContext.RemoveRange(childItems.Where(o => o.ChannelInfo is not null).Select(o => o.ChannelInfo));
                }
                break;
            case LogType.ChannelUpdated:
                {
                    var childItems = await RemoveChildDataAsync<ChannelUpdated>(item, q => q.Include(o => o.After).Include(o => o.Before));
                    DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                    DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                }
                break;
            case LogType.EmoteDeleted:
                await RemoveChildDataAsync<DeletedEmote>(item);
                break;
            case LogType.OverwriteCreated:
                {
                    var childItems = await RemoveChildDataAsync<OverwriteCreated>(item, q => q.Include(o => o.OverwriteInfo));
                    DbContext.RemoveRange(childItems.Where(o => o.OverwriteInfo is not null).Select(o => o.OverwriteInfo));
                }
                break;
            case LogType.OverwriteDeleted:
                {
                    var childItems = await RemoveChildDataAsync<OverwriteDeleted>(item, q => q.Include(o => o.OverwriteInfo));
                    DbContext.RemoveRange(childItems.Where(o => o.OverwriteInfo is not null).Select(o => o.OverwriteInfo));
                }
                break;
            case LogType.OverwriteUpdated:
                {
                    var childItems = await RemoveChildDataAsync<OverwriteUpdated>(item, q => q.Include(o => o.After).Include(o => o.Before));
                    DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                    DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                }
                break;
            case LogType.Unban:
                await RemoveChildDataAsync<Unban>(item);
                break;
            case LogType.MemberUpdated:
                {
                    var childItems = await RemoveChildDataAsync<MemberUpdated>(item, q => q.Include(o => o.After).Include(o => o.Before));
                    DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                    DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                }
                break;
            case LogType.GuildUpdated:
                {
                    var childItems = await RemoveChildDataAsync<GuildUpdated>(item, q => q.Include(o => o.After).Include(o => o.Before));
                    DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                    DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
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
                        DbContext.Remove(childItem);
                        foreach (var embed in childItem.Embeds)
                        {
                            DbContext.Remove(embed);
                            foreach (var field in embed.Fields)
                                DbContext.Remove(field);
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
                    DbContext.RemoveRange(childItems.Where(o => o.ThreadInfo is not null).Select(o => o.ThreadInfo));
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
                    DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                    DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                }
                break;
            case LogType.RoleDeleted:
                {
                    var childItems = await RemoveChildDataAsync<RoleDeleted>(item, q => q.Include(o => o.RoleInfo));
                    DbContext.RemoveRange(childItems.Where(o => o.RoleInfo is not null).Select(o => o.RoleInfo));
                }
                break;
        }
    }

    private async Task EnqueueFileDeletionAsync(LogItem item)
    {
        if (item.Files.Count == 0)
            return;

        var batch = item.Files.Select(f => new FileDeletePayload(f.Filename)).ToList();
        await Publisher.PublishBatchAsync(batch);
    }

    private async Task<List<TChildType>> RemoveChildDataAsync<TChildType>(LogItem parent, Func<IQueryable<TChildType>, IQueryable<TChildType>>? includeAction = null) where TChildType : ChildEntityBase
    {
        var query = DbContext.Set<TChildType>().Where(o => o.LogItemId == parent.Id);
        if (includeAction is not null)
            query = includeAction(query);

        var childItem = await ContextHelper.ReadEntitiesAsync(query);
        DbContext.RemoveRange(childItem);

        return childItem;
    }
}
