using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models;

namespace PointsService.Core.Repository;

public class TransactionRepository : RepositoryBase<PointsServiceContext>
{
    public TransactionRepository(PointsServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
    }

    private IQueryable<Transaction> GetBaseQuery(string guildId, bool disableTracking = false)
    {
        var query = Context.Transactions.Where(o => o.GuildId == guildId);
        if (disableTracking)
            query = query.AsNoTracking();

        return query;
    }

    public async Task<bool> ExistsTransactionAsync(string guildId, string messageId, string userId, string reactionId = "")
    {
        using (CreateCounter())
        {
            return await GetBaseQuery(guildId, true)
                .AnyAsync(o => o.ReactionId == reactionId && o.MessageId == messageId && o.UserId == userId);
        }
    }

    public async Task<List<Transaction>> FindTransactionsAsync(string guildId, string messageId, string? reactionId = null)
    {
        using (CreateCounter())
        {
            var query = GetBaseQuery(guildId).Where(o => o.MergedCount == 0 && o.MessageId == messageId);
            if (!string.IsNullOrEmpty(reactionId))
                query = query.Where(o => o.ReactionId == reactionId);
            return await query.ToListAsync();
        }
    }

    public async Task<int> ComputePointsStatusAsync(string guildId, string userId, bool expired, DateTime dateFrom, DateTime dateTo)
    {
        using (CreateCounter())
        {
            var query = Context.Transactions.AsNoTracking().Where(o => o.GuildId == guildId && o.UserId == userId);
            query = expired ? query.Where(o => o.MergedCount > 0) : query.Where(o => o.MergedCount == 0);

            if (dateFrom != DateTime.MinValue)
                query = query.Where(o => o.CreatedAt >= dateFrom);
            if (dateTo != DateTime.MaxValue)
                query = query.Where(o => o.CreatedAt < dateTo);

            return await query.SumAsync(o => o.Value);
        }
    }

    public async Task<List<BoardItem>> ComputeLeaderboardAsync(string guildId, int skip, int count)
    {
        using (CreateCounter())
        {
            var endOfDay = new TimeSpan(0, 23, 59, 59, 999);
            var now = DateTime.UtcNow;

            var query = Context.Transactions.AsNoTracking().Where(o => o.MergedCount == 0 && o.GuildId == guildId);
            var boardQuery = query.GroupBy(o => o.UserId)
                .Select(o => new BoardItem
                {
                    UserId = o.Key,
                    Today = o.Where(x => x.CreatedAt >= now.Date && x.CreatedAt <= now.Date.Add(endOfDay)).Sum(x => x.Value),
                    Total = o.Sum(x => x.Value),
                    MonthBack = o.Where(x => x.CreatedAt >= now.AddMonths(-1)).Sum(x => x.Value),
                    YearBack = o.Where(x => x.CreatedAt >= now.AddYears(-1)).Sum(x => x.Value)
                })
                .OrderByDescending(o => o.YearBack)
                .AsQueryable();

            if (skip > 0) boardQuery = boardQuery.Skip(skip);
            if (count > 0) boardQuery = boardQuery.Take(count);

            return await boardQuery.ToListAsync();
        }
    }

    public async Task<List<PointsChartItem>> ComputeChartDataAsync(AdminListRequest request)
    {
        using (CreateCounter())
        {
            var query = CreateQuery(request, true)
                .GroupBy(o => o.CreatedAt.Date);

            var groupedQuery = query.Select(o => new PointsChartItem
            {
                Day = new DateOnly(o.Key.Year, o.Key.Month, o.Key.Day),
                MessagePoints = o.Where(x => x.ReactionId == "").Sum(x => x.Value),
                ReactionPoints = o.Where(x => x.ReactionId != "").Sum(x => x.Value)
            });

            return await groupedQuery.ToListAsync();
        }
    }

    public async Task<PaginatedResponse<Transaction>> GetAdminListAsync(AdminListRequest request)
    {
        using (CreateCounter())
        {
            var query = CreateQuery(request, true);
            return await PaginatedResponse<Transaction>.CreateWithEntityAsync(query, request.Pagination);
        }
    }

    public async Task<int> ComputePositionAsync(string guildId, int currentUserPoints, DateTime after)
    {
        using (CreateCounter())
        {
            var query = Context.Transactions.AsNoTracking()
                .Where(o => o.MergedCount == 0 && o.GuildId == guildId && o.CreatedAt >= after)
                .GroupBy(o => o.UserId)
                .Where(o => o.Sum(x => x.Value) > currentUserPoints);

            var count = await query.CountAsync();
            return count + 1;
        }
    }
}
