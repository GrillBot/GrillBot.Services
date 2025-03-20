using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core;
using PointsService.Core.Entity;
using PointsService.Models;

namespace PointsService.Actions;

public class GetChartAction(
    ICounterManager counterManager,
    PointsServiceContext dbContext,
    IRabbitPublisher publisher
) : ApiAction(counterManager, dbContext, publisher)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (AdminListRequest)Parameters[0]!;

        if (request.CreatedFrom is null || request.CreatedTo is null)
        {
            var (from, to) = await ComputeTransactionsDateAsync(request);

            request.CreatedFrom = from;
            request.CreatedTo = to.AddDays(1);
        }

        var showEverything = !request.OnlyReactions && !request.OnlyMessages;
        var showMessages = showEverything || request.OnlyMessages;
        var showReactions = showEverything || request.OnlyReactions;

        var query = CreateDailyStatsQuery(request)
            .GroupBy(o => o.Date)
            .Select(o => new PointsChartItem
            {
                Day = o.Key,
                MessagePoints = showMessages ? o.Sum(x => x.MessagePoints) : 0,
                ReactionPoints = showReactions ? o.Sum(x => x.ReactionPoints) : 0
            });

        var items = await ContextHelper.ReadEntitiesAsync(query);
        return ApiResult.Ok(items);
    }

    private async Task<(DateTime from, DateTime to)> ComputeTransactionsDateAsync(AdminListRequest request)
    {
        var query = CreateQuery(request)
            .GroupBy(_ => 1)
            .Select(o => new
            {
                Min = o.Min(x => x.CreatedAt.Date),
                Max = o.Max(x => x.CreatedAt.Date)
            });

        var result = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
        if (result is null)
            return (DateTime.UtcNow.Date, DateTime.UtcNow.Date);
        return (result.Min, result.Max);
    }

    private IQueryable<Transaction> CreateQuery(AdminListRequest request)
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

        return query;
    }

    private IQueryable<DailyStat> CreateDailyStatsQuery(AdminListRequest request)
    {
        var from = DateOnly.FromDateTime(request.CreatedFrom!.Value);
        var to = DateOnly.FromDateTime(request.CreatedTo!.Value);

        var query = DbContext.DailyStats.AsNoTracking()
            .Where(o => o.Date >= from && o.Date <= to)
            .OrderBy(o => o.Date)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.GuildId))
            query = query.Where(o => o.GuildId == request.GuildId);

        if (!string.IsNullOrEmpty(request.UserId))
            query = query.Where(o => o.UserId == request.UserId);

        return query;
    }
}
