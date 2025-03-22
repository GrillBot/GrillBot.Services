using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Core.Extensions;
using AuditLogService.Managers;
using AuditLogService.Models.Events;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers;

public class BulkDeleteEventHandler(
    ILoggerFactory loggerFactory,
    AuditLogServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher,
    DataRecalculationManager _dataRecalculation
) : BaseEventHandlerWithDb<BulkDeletePayload, AuditLogServiceContext>(loggerFactory, dbContext, counterManager, publisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(BulkDeletePayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var logItems = await ReadLogItemsAsync(message.Ids);
        var filesForDeletion = new List<FileDeletePayload>();

        foreach (var chunk in logItems.Chunk(100))
        {
            DbContext.RemoveRange(chunk);

            await DeleteChildDataAsync(chunk);
            filesForDeletion.AddRange(chunk.Where(o => o.Files.Count > 0).SelectMany(o => o.Files).Select(o => new FileDeletePayload(o.Filename)));
        }

        await ContextHelper.SaveChagesAsync();
        await _dataRecalculation.EnqueueRecalculationAsync(logItems);

        if (filesForDeletion.Count > 0)
            await Publisher.PublishAsync(filesForDeletion);
        return RabbitConsumptionResult.Success;
    }

    private async Task<List<LogItem>> ReadLogItemsAsync(List<Guid> ids)
    {
        var result = new List<LogItem>();
        var baseQuery = DbContext.LogItems.Include(o => o.Files);

        foreach (var chunk in ids.Distinct().Chunk(100))
        {
            var query = baseQuery.Where(o => chunk.Contains(o.Id));
            var logItems = await ContextHelper.ReadEntitiesAsync(query);

            foreach (var typeGroup in logItems.GroupBy(o => o.Type))
            {
                switch (typeGroup.Key)
                {
                    case LogType.Api:
                        await typeGroup.SetLogDataAsync(DbContext.ApiRequests, (logItem, data) => logItem.ApiRequest = data, ContextHelper, true);
                        break;
                    case LogType.InteractionCommand:
                        await typeGroup.SetLogDataAsync(DbContext.InteractionCommands, (logItems, data) => logItems.InteractionCommand = data, ContextHelper, true);
                        break;
                    case LogType.JobCompleted:
                        await typeGroup.SetLogDataAsync(DbContext.JobExecutions, (logItems, data) => logItems.Job = data, ContextHelper, true);
                        break;
                }
            }

            result.AddRange(logItems);
        }

        return result;
    }

    private async Task DeleteChildDataAsync(LogItem[] chunk)
    {
        foreach (var type in chunk.GroupBy(o => o.Type))
        {
            var logItemIds = type.Select(o => o.Id).ToList();

            switch (type.Key)
            {
                case LogType.Info or LogType.Warning or LogType.Error:
                    await ExecuteChildDataHardDeletion<LogMessage>(logItemIds);
                    break;
                case LogType.ChannelCreated:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<ChannelCreated>(logItemIds, q => q.Include(o => o.ChannelInfo));
                        DbContext.RemoveRange(childItems.Where(o => o.ChannelInfo is not null).Select(o => o.ChannelInfo));
                    }
                    break;
                case LogType.ChannelDeleted:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<ChannelDeleted>(logItemIds, q => q.Include(o => o.ChannelInfo));
                        DbContext.RemoveRange(childItems.Where(o => o.ChannelInfo is not null).Select(o => o.ChannelInfo));
                    }
                    break;
                case LogType.ChannelUpdated:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<ChannelUpdated>(logItemIds, q => q.Include(o => o.After).Include(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.EmoteDeleted:
                    await ExecuteChildDataHardDeletion<DeletedEmote>(logItemIds);
                    break;
                case LogType.OverwriteCreated:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<OverwriteCreated>(logItemIds, q => q.Include(o => o.OverwriteInfo));
                        DbContext.RemoveRange(childItems.Where(o => o.OverwriteInfo is not null).Select(o => o.OverwriteInfo));
                    }
                    break;
                case LogType.OverwriteDeleted:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<OverwriteDeleted>(logItemIds, q => q.Include(o => o.OverwriteInfo));
                        DbContext.RemoveRange(childItems.Where(o => o.OverwriteInfo is not null).Select(o => o.OverwriteInfo));
                    }
                    break;
                case LogType.OverwriteUpdated:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<OverwriteUpdated>(logItemIds, q => q.Include(o => o.After).Include(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.Unban:
                    await ExecuteChildDataHardDeletion<Unban>(logItemIds);
                    break;
                case LogType.MemberUpdated:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<MemberUpdated>(logItemIds, q => q.Include(o => o.After).Include(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.GuildUpdated:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<GuildUpdated>(logItemIds, q => q.Include(o => o.After).Include(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.UserLeft:
                    await ExecuteChildDataHardDeletion<UserLeft>(logItemIds);
                    break;
                case LogType.UserJoined:
                    await ExecuteChildDataHardDeletion<UserJoined>(logItemIds);
                    break;
                case LogType.MessageEdited:
                    await ExecuteChildDataHardDeletion<MessageEdited>(logItemIds);
                    break;
                case LogType.MessageDeleted:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<MessageDeleted>(logItemIds, q => q.Include(o => o.Embeds).ThenInclude(o => o.Fields));
                        foreach (var embed in childItems.SelectMany(o => o.Embeds))
                        {
                            DbContext.Remove(embed);
                            foreach (var field in embed.Fields)
                                DbContext.Remove(field);
                        }
                    }
                    break;
                case LogType.InteractionCommand:
                    await ExecuteChildDataHardDeletion<InteractionCommand>(logItemIds);
                    break;
                case LogType.ThreadDeleted:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<ThreadDeleted>(logItemIds, q => q.Include(o => o.ThreadInfo));
                        DbContext.RemoveRange(childItems.Where(o => o.ThreadInfo is not null).Select(o => o.ThreadInfo));
                    }
                    break;
                case LogType.JobCompleted:
                    await ExecuteChildDataHardDeletion<JobExecution>(logItemIds);
                    break;
                case LogType.Api:
                    await ExecuteChildDataHardDeletion<ApiRequest>(logItemIds);
                    break;
                case LogType.ThreadUpdated:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<ThreadUpdated>(logItemIds, q => q.Include(o => o.After).Include(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.Before is not null).Select(o => o.Before));
                        DbContext.RemoveRange(childItems.Where(o => o.After is not null).Select(o => o.After));
                    }
                    break;
                case LogType.RoleDeleted:
                    {
                        var childItems = await ExecuteChildDataSoftDeletion<RoleDeleted>(logItemIds, q => q.Include(o => o.RoleInfo));
                        DbContext.RemoveRange(childItems.Where(o => o.RoleInfo is not null).Select(o => o.RoleInfo));
                    }
                    break;
            }
        }
    }

    private async Task<List<TChildEntity>> ExecuteChildDataSoftDeletion<TChildEntity>(List<Guid> logItemIds, Func<IQueryable<TChildEntity>, IQueryable<TChildEntity>> includeData) where TChildEntity : ChildEntityBase
    {
        var query = DbContext.Set<TChildEntity>().Where(o => logItemIds.Contains(o.LogItemId));
        query = includeData(query);

        var childItems = await ContextHelper.ReadEntitiesAsync(query);
        DbContext.RemoveRange(childItems);

        return childItems;
    }

    private async Task ExecuteChildDataHardDeletion<TChildEntity>(List<Guid> logItemIds) where TChildEntity : ChildEntityBase
    {
        var query = DbContext.Set<TChildEntity>().Where(o => logItemIds.Contains(o.LogItemId));
        await ContextHelper.ExecuteBatchDeleteAsync(query);
    }
}
