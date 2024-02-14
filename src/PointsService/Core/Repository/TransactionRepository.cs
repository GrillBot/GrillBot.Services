using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class TransactionRepository : SubRepositoryBase<PointsServiceContext>
{
    public TransactionRepository(PointsServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
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
}
