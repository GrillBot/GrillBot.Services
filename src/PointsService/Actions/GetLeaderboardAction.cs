using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core;
using PointsService.Core.Entity;
using PointsService.Enums;
using PointsService.Models;

namespace PointsService.Actions;

public class GetLeaderboardAction : ApiAction
{
    public GetLeaderboardAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher)
        : base(counterManager, dbContext, publisher)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (LeaderboardRequest)Parameters[0]!;

        var query = DbContext.Leaderboard.Where(o => o.GuildId == request.GuildId);
        query = WithSortInQuery(query, request.Sort);
        query = WithQueryMapping(query, request.Columns);
        query = WithQueryPagination(query, request.Skip, request.Count);

        var data = await ReadLeaderboardFromQueryAsync(query);
        var result = data.ConvertAll(o => new BoardItem
        {
            MonthBack = o.MonthBack,
            Today = o.Today,
            Total = o.Total,
            UserId = o.UserId,
            YearBack = o.YearBack
        });

        return ApiResult.Ok(result);
    }

    private static IQueryable<LeaderboardItem> WithSortInQuery(IQueryable<LeaderboardItem> query, LeaderboardSortOptions sortOptions)
    {
        return sortOptions switch
        {
            LeaderboardSortOptions.ByTotalDescending => query.OrderByDescending(o => o.Total),
            _ => query.OrderByDescending(o => o.YearBack)
        };
    }

    private static IQueryable<LeaderboardItem> WithQueryMapping(IQueryable<LeaderboardItem> query, LeaderboardColumnFlag columns)
    {
        return query.Select(o => new LeaderboardItem
        {
            GuildId = o.GuildId,
            MonthBack = (columns & LeaderboardColumnFlag.MonthBack) != LeaderboardColumnFlag.None ? o.MonthBack : 0,
            Today = (columns & LeaderboardColumnFlag.Today) != LeaderboardColumnFlag.None ? o.Today : 0,
            Total = (columns & LeaderboardColumnFlag.Total) != LeaderboardColumnFlag.None ? o.Total : 0,
            UserId = o.UserId,
            YearBack = (columns & LeaderboardColumnFlag.YearBack) != LeaderboardColumnFlag.None ? o.YearBack : 0
        });
    }

    private static IQueryable<LeaderboardItem> WithQueryPagination(IQueryable<LeaderboardItem> query, int skip, int take)
    {
        if (skip > 0) query = query.Skip(skip);
        if (take > 0) query = query.Take(take);

        return query;
    }

    private async Task<List<LeaderboardItem>> ReadLeaderboardFromQueryAsync(IQueryable<LeaderboardItem> query)
        => await ContextHelper.ReadEntitiesAsync(query.AsNoTracking());
}
