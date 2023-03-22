using System.Linq.Expressions;
using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

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
}
