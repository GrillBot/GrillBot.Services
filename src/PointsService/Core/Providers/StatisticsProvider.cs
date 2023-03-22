using GrillBot.Core.Database;
using PointsService.Core.Repository;

namespace PointsService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    private PointsServiceRepository Repository { get; }

    public StatisticsProvider(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public Task<Dictionary<string, long>> GetTableStatisticsAsync()
        => Repository.Statistics.GetStatisticsAsync();
}
