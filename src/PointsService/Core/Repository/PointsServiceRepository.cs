using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class PointsServiceRepository : RepositoryBase<PointsServiceContext>
{
    public ChannelRepository Channel { get; }
    public UserRepository User { get; }
    public TransactionRepository Transaction { get; }
    public StatisticsRepository Statistics { get; }
    public LeaderboardRepository Leaderboard { get; }
    public DailyStatsRepository DailyStats { get; }

    public PointsServiceRepository(PointsServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
        Channel = new ChannelRepository(context, counterManager);
        User = new UserRepository(context, counterManager);
        Transaction = new TransactionRepository(context, counterManager);
        Statistics = new StatisticsRepository(context, counterManager);
        Leaderboard = new LeaderboardRepository(context, counterManager);
        DailyStats = new DailyStatsRepository(context, counterManager);
    }
}
