using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using EmoteService.Models.Request;
using EmoteService.Models.Response;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace EmoteService.Actions.Statistics;

public class GetEmoteStatisticsListAction : ApiAction<EmoteServiceContext>
{
    public GetEmoteStatisticsListAction(ICounterManager counterManager, EmoteServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (EmoteStatisticsListRequest)Parameters[0]!;

        var groupedQuery = CreateGroupedQuery(request);
        groupedQuery = CreateFilter(groupedQuery, request);
        groupedQuery = CreateSorting(groupedQuery, request.Sort);

        var result = await ContextHelper.ReadEntitiesWithPaginationAsync(groupedQuery, request.Pagination);

        if (!request.Unsupported)
        {
            foreach (var item in result.Data)
                item.EmoteUrl = CreateEmoteUrl(item.EmoteId, item.EmoteName, item.EmoteIsAnimated);
        }

        return ApiResult.Ok(result);
    }

    private IQueryable<EmoteStatisticsItem> CreateGroupedQuery(EmoteStatisticsListRequest request)
    {
        var query = DbContext.EmoteUserStatItems.AsNoTracking();

        if (request.Unsupported)
            query = query.Where(e => !DbContext.EmoteDefinitions.Any(d => d.EmoteIsAnimated == e.EmoteIsAnimated && d.EmoteId == e.EmoteId && d.EmoteName == e.EmoteName));
        else
            query = query.Where(e => DbContext.EmoteDefinitions.Any(d => d.EmoteIsAnimated == e.EmoteIsAnimated && d.EmoteId == e.EmoteId && d.EmoteName == e.EmoteName));

        if (!string.IsNullOrEmpty(request.GuildId))
            query = query.Where(o => o.GuildId == request.GuildId);

        if (request.IgnoreAnimated)
            query = query.Where(o => !o.EmoteIsAnimated);

        if (!string.IsNullOrEmpty(request.EmoteFullId))
            query = query.WithEmoteQuery(Emote.Parse(request.EmoteFullId));
        else if (!string.IsNullOrEmpty(request.EmoteName))
            query = query.Where(o => EF.Functions.ILike(o.EmoteName, $"{request.EmoteName}%"));

        return query
            .GroupBy(o => new { o.GuildId, o.EmoteId, o.EmoteIsAnimated, o.EmoteName })
            .Select(o => new EmoteStatisticsItem
            {
                EmoteId = o.Key.EmoteId,
                EmoteName = o.Key.EmoteName,
                EmoteIsAnimated = o.Key.EmoteIsAnimated,
                GuildId = o.Key.GuildId,
                FirstOccurence = o.Min(x => x.FirstOccurence),
                LastOccurence = o.Max(x => x.LastOccurence),
                UseCount = o.Sum(x => x.UseCount),
                UsersCount = o.Count()
            });
    }

    private static IQueryable<EmoteStatisticsItem> CreateFilter(IQueryable<EmoteStatisticsItem> query, EmoteStatisticsListRequest request)
    {
        if (request.UseCountFrom is > 0)
            query = query.Where(o => o.UseCount >= request.UseCountFrom.Value);
        if (request.UseCountTo is > 0)
            query = query.Where(o => o.UseCount <= request.UseCountTo.Value);

        if (request.FirstOccurenceFrom is not null && request.FirstOccurenceFrom != DateTime.MinValue)
            query = query.Where(o => o.FirstOccurence >= request.FirstOccurenceFrom.Value);
        if (request.FirstOccurenceTo is not null && request.FirstOccurenceTo != DateTime.MinValue)
            query = query.Where(o => o.FirstOccurence <= request.FirstOccurenceTo.Value);

        if (request.LastOccurenceFrom is not null && request.LastOccurenceFrom != DateTime.MinValue)
            query = query.Where(o => o.LastOccurence >= request.LastOccurenceFrom.Value);
        if (request.LastOccurenceTo is not null && request.LastOccurenceTo != DateTime.MinValue)
            query = query.Where(o => o.LastOccurence <= request.LastOccurenceTo.Value);

        return query;
    }

    private static IQueryable<EmoteStatisticsItem> CreateSorting(IQueryable<EmoteStatisticsItem> query, SortParameters parameters)
    {
        var expressions = parameters.OrderBy switch
        {
            "FirstOccurence" => new Expression<Func<EmoteStatisticsItem, object>>[] { entity => entity.FirstOccurence },
            "LastOccurence" => new Expression<Func<EmoteStatisticsItem, object>>[] { entity => entity.LastOccurence },
            "UsersCount" => new Expression<Func<EmoteStatisticsItem, object>>[] { entity => entity.UsersCount },
            _ => new Expression<Func<EmoteStatisticsItem, object>>[] { entity => entity.UseCount }
        };

        return query.WithSorting(expressions, parameters.Descending);
    }

    private static string CreateEmoteUrl(string id, string name, bool animated)
    {
        var definition = new Core.Entity.EmoteDefinition
        {
            EmoteId = id,
            EmoteIsAnimated = animated,
            EmoteName = name
        };

        return Emote.Parse(definition.ToString()).Url;
    }
}
