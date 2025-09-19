using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Core.Extensions;
using AuditLogService.Managers;
using AuditLogService.Models.Events;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers;

public class BulkDeleteEventHandler(
    IServiceProvider serviceProvider,
    DataRecalculationManager _dataRecalculation
) : BaseEventHandlerWithDb<BulkDeletePayload, AuditLogServiceContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(BulkDeletePayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        foreach (var chunk in message.Ids.Distinct().Chunk(100))
        {
            var logItems = await ReadLogItemsAsync(chunk, cancellationToken);
            if (logItems.Count == 0)
                continue;

            DbContext.RemoveRange(logItems);
            await DeleteChildDataAsync(logItems, cancellationToken);

            var filesForDelete = logItems.SelectMany(o => o.Files).Select(o => new FileDeletePayload(o.Filename)).ToList();

            await ContextHelper.SaveChangesAsync(cancellationToken);
            await _dataRecalculation.EnqueueRecalculationAsync(logItems, cancellationToken);

            if (filesForDelete.Count > 0)
                await Publisher.PublishAsync(filesForDelete, cancellationToken: cancellationToken);
        }

        return RabbitConsumptionResult.Success;
    }

    private async Task<List<LogItem>> ReadLogItemsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var result = new List<LogItem>();
        var baseQuery = DbContext.LogItems.Include(o => o.Files);

        foreach (var chunk in ids.Distinct().Chunk(100))
        {
            var query = baseQuery.Where(o => chunk.Contains(o.Id));
            var logItems = await ContextHelper.ReadEntitiesAsync(query, cancellationToken);

            foreach (var typeGroup in logItems.GroupBy(o => o.Type))
            {
                switch (typeGroup.Key)
                {
                    case LogType.Api:
                        await typeGroup.SetLogDataAsync(DbContext.ApiRequests, (logItem, data) => logItem.ApiRequest = data, ContextHelper, true, cancellationToken);
                        break;
                    case LogType.InteractionCommand:
                        await typeGroup.SetLogDataAsync(DbContext.InteractionCommands, (logItems, data) => logItems.InteractionCommand = data, ContextHelper, true, cancellationToken);
                        break;
                    case LogType.JobCompleted:
                        await typeGroup.SetLogDataAsync(DbContext.JobExecutions, (logItems, data) => logItems.Job = data, ContextHelper, true, cancellationToken);
                        break;
                }
            }

            result.AddRange(logItems);
        }

        return result;
    }

    private async Task DeleteChildDataAsync(IEnumerable<LogItem> chunk, CancellationToken cancellationToken = default)
    {
        foreach (var type in chunk.GroupBy(o => o.Type))
        {
            var logItemIds = type.Select(o => o.Id).ToList();

            switch (type.Key)
            {
                case LogType.Info or LogType.Warning or LogType.Error:
                    ExecuteDeletion<LogMessage>(logItemIds);
                    break;
                case LogType.ChannelCreated:
                    {
                        var childItems = await ExecuteDeletionAsync<ChannelCreated>(logItemIds, q => q.Include(o => o.ChannelInfo), cancellationToken);
                        DbContext.RemoveRange(childItems.Where(o => o.ChannelInfo is not null).Select(o => o.ChannelInfo));
                    }
                    break;
                case LogType.ChannelDeleted:
                    {
                        var childItems = await ExecuteDeletionAsync<ChannelDeleted>(logItemIds, q => q.Include(o => o.ChannelInfo), cancellationToken);
                        DbContext.RemoveRange(childItems.Where(o => o.ChannelInfo is not null).Select(o => o.ChannelInfo));
                    }
                    break;
                case LogType.ChannelUpdated:
                    {
                        var childItems = await ExecuteDeletionAsync<ChannelUpdated>(logItemIds, q => q.Include(o => o.After).Include(o => o.Before), cancellationToken);
                        DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.EmoteDeleted:
                    ExecuteDeletion<DeletedEmote>(logItemIds);
                    break;
                case LogType.OverwriteCreated:
                    {
                        var childItems = await ExecuteDeletionAsync<OverwriteCreated>(logItemIds, q => q.Include(o => o.OverwriteInfo), cancellationToken);
                        DbContext.RemoveRange(childItems.Where(o => o.OverwriteInfo is not null).Select(o => o.OverwriteInfo));
                    }
                    break;
                case LogType.OverwriteDeleted:
                    {
                        var childItems = await ExecuteDeletionAsync<OverwriteDeleted>(logItemIds, q => q.Include(o => o.OverwriteInfo), cancellationToken);
                        DbContext.RemoveRange(childItems.Where(o => o.OverwriteInfo is not null).Select(o => o.OverwriteInfo));
                    }
                    break;
                case LogType.OverwriteUpdated:
                    {
                        var childItems = await ExecuteDeletionAsync<OverwriteUpdated>(logItemIds, q => q.Include(o => o.After).Include(o => o.Before), cancellationToken);
                        DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.Unban:
                    ExecuteDeletion<Unban>(logItemIds);
                    break;
                case LogType.MemberUpdated:
                    {
                        var childItems = await ExecuteDeletionAsync<MemberUpdated>(logItemIds, q => q.Include(o => o.After).Include(o => o.Before), cancellationToken);
                        DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.GuildUpdated:
                    {
                        var childItems = await ExecuteDeletionAsync<GuildUpdated>(logItemIds, q => q.Include(o => o.After).Include(o => o.Before), cancellationToken);
                        DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.UserLeft:
                    ExecuteDeletion<UserLeft>(logItemIds);
                    break;
                case LogType.UserJoined:
                    ExecuteDeletion<UserJoined>(logItemIds);
                    break;
                case LogType.MessageEdited:
                    ExecuteDeletion<MessageEdited>(logItemIds);
                    break;
                case LogType.MessageDeleted:
                    {
                        var childItems = await ExecuteDeletionAsync<MessageDeleted>(logItemIds, q => q.Include(o => o.Embeds).ThenInclude(o => o.Fields), cancellationToken);
                        foreach (var embed in childItems.SelectMany(o => o.Embeds))
                        {
                            DbContext.Remove(embed);
                            foreach (var field in embed.Fields)
                                DbContext.Remove(field);
                        }
                    }
                    break;
                case LogType.InteractionCommand:
                    ExecuteDeletion<InteractionCommand>(logItemIds);
                    break;
                case LogType.ThreadDeleted:
                    {
                        var childItems = await ExecuteDeletionAsync<ThreadDeleted>(logItemIds, q => q.Include(o => o.ThreadInfo), cancellationToken);
                        DbContext.RemoveRange(childItems.Where(o => o.ThreadInfo is not null).Select(o => o.ThreadInfo));
                    }
                    break;
                case LogType.JobCompleted:
                    ExecuteDeletion<JobExecution>(logItemIds);
                    break;
                case LogType.Api:
                    ExecuteDeletion<ApiRequest>(logItemIds);
                    break;
                case LogType.ThreadUpdated:
                    {
                        var childItems = await ExecuteDeletionAsync<ThreadUpdated>(logItemIds, q => q.Include(o => o.After).Include(o => o.Before), cancellationToken);
                        DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.RoleDeleted:
                    {
                        var childItems = await ExecuteDeletionAsync<RoleDeleted>(logItemIds, q => q.Include(o => o.RoleInfo), cancellationToken);
                        DbContext.RemoveRange(childItems.Where(o => o.RoleInfo is not null).Select(o => o.RoleInfo));
                    }
                    break;
            }
        }
    }

    private async Task<List<TChildEntity>> ExecuteDeletionAsync<TChildEntity>(
        List<Guid> logItemIds,
        Func<IQueryable<TChildEntity>, IQueryable<TChildEntity>> includeData,
        CancellationToken cancellationToken = default
    ) where TChildEntity : ChildEntityBase
    {
        var query = DbContext.Set<TChildEntity>().Where(o => logItemIds.Contains(o.LogItemId));
        query = includeData(query);

        var childItems = await ContextHelper.ReadEntitiesAsync(query, cancellationToken);
        DbContext.RemoveRange(childItems);

        return childItems;
    }

    private void ExecuteDeletion<TChildEntity>(List<Guid> logItemIds) where TChildEntity : ChildEntityBase, new()
    {
        foreach (var id in logItemIds)
            DbContext.Entry(new TChildEntity() { LogItemId = id }).State = EntityState.Deleted;
    }
}
