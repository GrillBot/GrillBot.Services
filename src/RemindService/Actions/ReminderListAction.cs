using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using RemindService.Core.Entity;
using RemindService.Models.Request;
using RemindService.Models.Response;
using System.Linq.Expressions;

namespace RemindService.Actions;

public class ReminderListAction(
    ICounterManager counterManager,
    RemindServiceContext dbContext
) : ApiAction<RemindServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<ReminderListRequest>(0);
        var reminders = await ReadRemindersAsync(request);

        var result = await PaginatedResponse<RemindMessageItem>.CopyAndMapAsync(
            reminders,
            entity => Task.FromResult(MapItem(entity))
        );

        return ApiResult.Ok(result);
    }

    private Task<PaginatedResponse<RemindMessage>> ReadRemindersAsync(ReminderListRequest request)
    {
        var query = DbContext.RemindMessages.AsNoTracking();

        if (!string.IsNullOrEmpty(request.FromUserId))
            query = query.Where(o => o.FromUserId == request.FromUserId);

        if (!string.IsNullOrEmpty(request.ToUserId))
            query = query.Where(o => o.ToUserId == request.ToUserId);

        if (!string.IsNullOrEmpty(request.CommandMessageId))
            query = query.Where(o => o.CommandMessageId == request.CommandMessageId);

        if (!string.IsNullOrEmpty(request.MessageContains))
            query = query.Where(o => EF.Functions.ILike(o.Message, $"%{request.MessageContains}%"));

        if (request.NotifyAtFromUtc.HasValue)
            query = query.Where(o => o.NotifyAtUtc >= request.NotifyAtFromUtc.Value);

        if (request.NotifyAtToUtc.HasValue)
            query = query.Where(o => o.NotifyAtUtc <= request.NotifyAtToUtc.Value);

        if (request.OnlyPending.HasValue)
        {
            if (request.OnlyPending.Value)
                query = query.Where(o => o.NotificationMessageId == null);
            else
                query = query.Where(o => o.NotificationMessageId != null);
        }

        if (request.OnlyInProcess.HasValue)
            query = query.Where(o => o.IsSendInProgress == request.OnlyInProcess.Value);

        query = CreateSorting(query, request.Sort);
        return ContextHelper.ReadEntitiesWithPaginationAsync(query, request.Pagination);
    }

    private static IQueryable<RemindMessage> CreateSorting(IQueryable<RemindMessage> query, SortParameters sort)
    {
        var expressions = sort.OrderBy switch
        {
            "NotifyAt" =>
            [
                entity => entity.NotifyAtUtc,
                entity => entity.Id
            ],
            "PostponeCount" =>
            [
                entity => entity.PostponeCount,
                entity => entity.Id
            ],
            _ => new Expression<Func<RemindMessage, object>>[] { entity => entity.Id }
        };

        return query.WithSorting(expressions, sort.Descending);
    }

    private static RemindMessageItem MapItem(RemindMessage message)
    {
        return new(
            message.Id,
            message.FromUserId,
            message.ToUserId,
            message.NotifyAtUtc,
            message.Message,
            message.PostponeCount,
            message.NotificationMessageId,
            message.CommandMessageId,
            message.Language,
            message.IsSendInProgress
        );
    }
}
