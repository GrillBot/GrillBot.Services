using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class StatisticsRepository : RepositoryBase
{
    public StatisticsRepository(PointsServiceContext context) : base(context)
    {
    }

    public async Task<Dictionary<string, long>> GetStatisticsAsync()
    {
        return new Dictionary<string, long>
        {
            { nameof(Context.Channels), await Context.Channels.LongCountAsync() },
            { nameof(Context.Users), await Context.Users.LongCountAsync() },
            { nameof(Context.Transactions), await Context.Transactions.LongCountAsync() }
        };
    }
}
