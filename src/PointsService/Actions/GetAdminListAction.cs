using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core;
using PointsService.Core.Entity;
using PointsService.Models;

namespace PointsService.Actions;

public class GetAdminListAction : ApiAction
{
    public GetAdminListAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher)
        : base(counterManager, dbContext, publisher)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (AdminListRequest)Parameters[0]!;
        var transactions = await ReadTransactionsAsync(request);

        var result = await PaginatedResponse<TransactionItem>.CopyAndMapAsync(transactions, entity => Task.FromResult(new TransactionItem
        {
            CreatedAt = entity.CreatedAt,
            MergedCount = entity.MergedCount,
            Value = entity.Value,
            GuildId = entity.GuildId,
            ReactionId = entity.ReactionId,
            MergedFrom = entity.MergeRangeFrom,
            MergedTo = entity.MergeRangeTo,
            MessageId = entity.MessageId,
            UserId = entity.UserId
        }));

        return ApiResult.Ok(result);
    }

    private async Task<PaginatedResponse<Transaction>> ReadTransactionsAsync(AdminListRequest request)
    {
        var query = DbContext.Transactions.AsNoTracking();

        if (request.ShowMerged)
            query = query.Where(o => o.MergedCount > 0);
        else
            query = query.Where(o => o.MergedCount == 0);

        if (!string.IsNullOrEmpty(request.GuildId))
            query = query.Where(o => o.GuildId == request.GuildId);

        if (!string.IsNullOrEmpty(request.UserId))
            query = query.Where(o => o.UserId == request.UserId);

        if (request.CreatedFrom.HasValue)
            query = query.Where(o => o.CreatedAt >= request.CreatedFrom.Value.ToUniversalTime());

        if (request.CreatedTo.HasValue)
            query = query.Where(o => o.CreatedAt < request.CreatedTo.Value.ToUniversalTime());

        if (request.OnlyMessages)
            query = query.Where(o => o.ReactionId == "");

        if (request.OnlyReactions)
            query = query.Where(o => o.ReactionId != "");

        if (!request.ShowMerged && !string.IsNullOrEmpty(request.MessageId))
            query = query.Where(o => o.MessageId == request.MessageId);

        if (request.Sort is not null)
        {
            query = request.Sort.OrderBy switch
            {
                "Value" => request.Sort.Descending ?
                    query.OrderByDescending(o => o.Value).ThenByDescending(o => o.CreatedAt) :
                    query.OrderBy(o => o.Value).ThenBy(o => o.CreatedAt),
                _ => request.Sort.Descending ?
                    query.OrderByDescending(o => o.CreatedAt) :
                    query.OrderBy(o => o.CreatedAt)
            };
        }

        return await ContextHelper.ReadEntitiesWithPaginationAsync(query, request.Pagination);
    }
}
