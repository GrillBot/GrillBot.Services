using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using SearchingService.Core.Entity;
using SearchingService.Models.Request;
using SearchingService.Models.Response;
using System.Linq.Expressions;

namespace SearchingService.Actions;

public class GetSearchingListAction : ApiAction<SearchingServiceContext>
{
    public GetSearchingListAction(ICounterManager counterManager, SearchingServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<SearchingListRequest>(0);
        var items = await ReadItemsAsync(request);

        var result = await PaginatedResponse<SearchListItem>.CopyAndMapAsync(
            items,
            entity => Task.FromResult(new SearchListItem(
                entity.Id,
                entity.UserId,
                entity.GuildId,
                entity.ChannelId,
                entity.Content,
                entity.CreatedAt,
                entity.ValidTo,
                entity.IsDeleted
            ))
        );

        return ApiResult.Ok(result);
    }

    private Task<PaginatedResponse<SearchItem>> ReadItemsAsync(SearchingListRequest request)
    {
        var query = DbContext.Items.AsNoTracking();

        if (!request.ShowDeleted)
            query = query.Where(o => !o.IsDeleted);

        if (!string.IsNullOrEmpty(request.UserId))
            query = query.Where(o => o.UserId == request.UserId);

        if (!string.IsNullOrEmpty(request.GuildId))
            query = query.Where(o => o.GuildId == request.GuildId);

        if (!string.IsNullOrEmpty(request.ChannelId))
            query = query.Where(o => o.ChannelId == request.ChannelId);

        if (!string.IsNullOrEmpty(request.MessageQuery))
            query = query.Where(o => EF.Functions.ILike(o.Content, $"%{request.MessageQuery}%"));

        if (request.CreatedFrom is not null)
            query = query.Where(o => o.CreatedAt >= request.CreatedFrom.Value);

        if (request.CreatedTo is not null)
            query = query.Where(o => o.CreatedAt <= request.CreatedTo.Value);

        if (request.HideInvalid)
        {
            query = query.Where(o => o.ValidTo >= DateTime.UtcNow);
        }
        else
        {
            if (request.ValidFrom is not null)
                query = query.Where(o => o.ValidTo >= request.ValidFrom);

            if (request.ValidTo is not null)
                query = query.Where(o => o.ValidTo <= request.ValidTo);
        }

        query = CreateSorting(query, request.Sort);
        return ContextHelper.ReadEntitiesWithPaginationAsync(query, request.Pagination);
    }

    private static IQueryable<SearchItem> CreateSorting(IQueryable<SearchItem> query, SortParameters sort)
    {
        var expressions = sort.OrderBy switch
        {
            "CreatedAt" => new Expression<Func<SearchItem, object>>[]
            {
                entity => entity.CreatedAt,
                entity => entity.Id
            },
            "ValidTo" => new Expression<Func<SearchItem, object>>[]
            {
                entity => entity.ValidTo,
                entity => entity.Id
            },
            _ => new Expression<Func<SearchItem, object>>[] { entity => entity.Id }
        };

        return query.WithSorting(expressions, sort.Descending);
    }
}
