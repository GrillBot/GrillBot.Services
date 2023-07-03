using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using RubbergodService.Core.Entity;

namespace RubbergodService.Core.Repository;

public class RubbergodServiceRepository : RepositoryBase<RubbergodServiceContext>
{
    public RubbergodServiceRepository(RubbergodServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
        Karma = new KarmaRepository(Context, counterManager);
        MemberCache = new MemberCacheRepository(Context, counterManager);
        Statistics = new StatisticsRepository(Context, counterManager);
        PinCache = new PinCacheRepository(Context, counterManager);
    }

    public KarmaRepository Karma { get; }
    public MemberCacheRepository MemberCache { get; }
    public StatisticsRepository Statistics { get; }
    public PinCacheRepository PinCache { get; }
}
