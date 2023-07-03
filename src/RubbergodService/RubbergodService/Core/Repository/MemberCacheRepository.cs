using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using RubbergodService.Core.Entity;

namespace RubbergodService.Core.Repository;

public class MemberCacheRepository : SubRepositoryBase<RubbergodServiceContext>
{
    public MemberCacheRepository(RubbergodServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
    }

    public async Task<MemberCacheItem?> FindMemberByIdAsync(string memberId)
    {
        using (CreateCounter())
        {
            return await Context.MemberCache
                .FirstOrDefaultAsync(o => o.UserId == memberId);
        }
    }
}
