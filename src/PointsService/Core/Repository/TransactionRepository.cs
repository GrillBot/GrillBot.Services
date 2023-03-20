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

    public async Task<bool> ExistsTransactionAsync(string guildId, string messageId, string reactionId = "")
    {
        using (CreateCounter())
        {
            return await Context.Transactions.AsNoTracking()
                .AnyAsync(o => o.GuildId == guildId && o.ReactionId == reactionId && o.MessageId == messageId);
        }
    }
}
