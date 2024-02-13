using GrillBot.Core.Database.Repository;
using GrillBot.Core.Helpers;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models;

namespace PointsService.Core.Repository;

public class TransactionRepository : SubRepositoryBase<PointsServiceContext>
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

    public async Task<bool> ExistsAnyTransactionAsync(string guildId, string userId)
    {
        using (CreateCounter())
        {
            return await GetBaseQuery(guildId, true)
                .AnyAsync(o => o.UserId == userId);
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

    public async Task<PointsStatus> ComputePointsStatusAsync(string guildId, string userId, bool expired)
    {
        var now = DateTime.UtcNow;
        var endOfToday = DateHelper.EndOfDayUtc;

        var query = GetBaseQuery(guildId, true)
            .Where(o => o.UserId == userId);

        var dtoQuery = query
            .GroupBy(_ => 1)
            .Select(transactions => new PointsStatus
            {
                Today = transactions.Where(expired ? t => t.MergedCount > 0 : t => t.MergedCount == 0).Where(o => o.CreatedAt >= now.Date && o.CreatedAt <= endOfToday).Sum(o => o.Value),
                Total = transactions.Sum(o => o.Value),
                MonthBack = transactions.Where(expired ? t => t.MergedCount > 0 : t => t.MergedCount == 0).Where(x => x.CreatedAt >= now.AddMonths(-1)).Sum(x => x.Value),
                YearBack = transactions.Where(expired ? t => t.MergedCount > 0 : t => t.MergedCount == 0).Where(x => x.CreatedAt >= now.AddYears(-1)).Sum(x => x.Value)
            });

        return (await dtoQuery.FirstOrDefaultAsync()) ?? new PointsStatus();
    }

    public async Task<PaginatedResponse<Transaction>> GetAdminListAsync(AdminListRequest request)
    {
        using (CreateCounter())
        {
            var query = CreateQuery(request, true);
            return await PaginatedResponse<Transaction>.CreateWithEntityAsync(query, request.Pagination);
        }
    }

    private IQueryable<Transaction> GetTransactionsForMergeQuery(DateTime expirationDate)
    {
        return Context.Transactions
            .Where(o => o.CreatedAt < expirationDate && o.MergedCount == 0);
    }

    public async Task<int> GetCountOfTransactionsForMergeAsync(DateTime expirationDate)
    {
        using (CreateCounter())
        {
            return await GetTransactionsForMergeQuery(expirationDate).CountAsync();
        }
    }

    public async Task<List<Transaction>> GetTransactionsForMergeAsync(DateTime expirationDate)
    {
        using (CreateCounter())
        {
            return await GetTransactionsForMergeQuery(expirationDate).ToListAsync();
        }
    }

    public async Task<(DateTime from, DateTime to)> ComputeTransactionDateRangeAsync(AdminListRequest request)
    {
        using (CreateCounter())
        {
            var query = CreateQuery(request, true)
                .GroupBy(o => 1)
                .Select(o => new
                {
                    Min = o.Min(x => x.CreatedAt.Date),
                    Max = o.Max(x => x.CreatedAt.Date)
                });

            var data = await query.FirstOrDefaultAsync();
            return data is null ? (DateTime.UtcNow.Date, DateTime.UtcNow.Date) : (data.Min, data.Max);
        }
    }
}
