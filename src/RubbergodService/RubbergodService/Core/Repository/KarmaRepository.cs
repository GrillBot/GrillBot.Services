using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using RubbergodService.Core.Entity;

namespace RubbergodService.Core.Repository;

public class KarmaRepository : SubRepositoryBase<RubbergodServiceContext>
{
    public KarmaRepository(RubbergodServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
    }

    public async Task<Karma?> FindKarmaByMemberIdAsync(string memberId)
    {
        using (CreateCounter())
        {
            return await Context.Karma
                .FirstOrDefaultAsync(o => o.MemberId == memberId);
        }
    }

    public async Task<PaginatedResponse<Karma>> GetKarmaPageAsync(PaginatedParams parameters)
    {
        using (CreateCounter())
        {
            var query = Context.Karma.AsNoTracking()
                .OrderByDescending(o => o.KarmaValue);

            return await PaginatedResponse<Karma>.CreateWithEntityAsync(query, parameters);
        }
    }
}
