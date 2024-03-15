using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using EmoteService.Models.Request;
using EmoteService.Models.Response;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmoteService.Actions.Statistics;

public class GetUserEmoteUsageListAction : ApiAction<EmoteServiceContext>
{
    public GetUserEmoteUsageListAction(ICounterManager counterManager, EmoteServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (EmoteUserUsageListRequest)Parameters[0]!;
        var emote = Emote.Parse(request.EmoteId);

        var query = DbContext.EmoteUserStatItems.AsNoTracking()
            .Where(o => o.GuildId == request.GuildId)
            .WithEmoteQuery(emote);

        query = CreateSorting(query, request.Sort);

        var paginatedEntities = await ReadPaginatedEntities(query, request.Pagination);
        var result = await PaginatedResponse<EmoteUserUsageItem>.CopyAndMapAsync(paginatedEntities, entity => Task.FromResult(MapEntity(entity)));

        return ApiResult.Ok(result);
    }

    private static IQueryable<EmoteUserStatItem> CreateSorting(IQueryable<EmoteUserStatItem> query, SortParameters parameters)
    {
        return parameters.OrderBy switch
        {
            "FirstOccurence" => query.CreateSortingOfParameter(item => item.FirstOccurence, parameters.Descending),
            "LastOccurence" => query.CreateSortingOfParameter(item => item.LastOccurence, parameters.Descending),
            _ => query.CreateSortingOfParameter(item => item.UseCount, parameters.Descending)
        };
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
