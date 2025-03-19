using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using EmoteService.Models.Request;
using EmoteService.Models.Response;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmoteService.Actions.Statistics;

public class GetUserEmoteUsageListAction(
    ICounterManager counterManager,
    EmoteServiceContext dbContext
) : ApiAction<EmoteServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (EmoteUserUsageListRequest)Parameters[0]!;
        var emote = Emote.Parse(request.EmoteId);

        var query = DbContext.EmoteUserStatItems.AsNoTracking()
            .Where(o => o.GuildId == request.GuildId)
            .WithEmoteQuery(emote);

        query = CreateSorting(query, request.Sort);

        var paginatedEntities = await ContextHelper.ReadEntitiesWithPaginationAsync(query, request.Pagination);
        var result = await PaginatedResponse<EmoteUserUsageItem>.CopyAndMapAsync(paginatedEntities, entity => Task.FromResult(MapEntity(entity)));

        return ApiResult.Ok(result);
    }

    private static IQueryable<EmoteUserStatItem> CreateSorting(IQueryable<EmoteUserStatItem> query, SortParameters parameters)
    {
        var expressions = parameters.OrderBy switch
        {
            "FirstOccurence" => [entity => entity.FirstOccurence],
            "LastOccurence" => [entity => entity.LastOccurence],
            _ => new Expression<Func<EmoteUserStatItem, object>>[] { entity => entity.UseCount },
        };

        return query.WithSorting(expressions, parameters.Descending);
    }

    private static EmoteUserUsageItem MapEntity(EmoteUserStatItem item)
    {
        return new EmoteUserUsageItem
        {
            FirstOccurence = item.FirstOccurence,
            LastOccurence = item.LastOccurence,
            UseCount = item.UseCount,
            UserId = item.UserId
        };
    }
}
