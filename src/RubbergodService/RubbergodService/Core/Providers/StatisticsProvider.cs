using GrillBot.Core.Database;
using RubbergodService.Core.Repository;

namespace RubbergodService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    private RubbergodServiceRepository Repository { get; }

    public StatisticsProvider(RubbergodServiceRepository repository)
    {
        Repository = repository;
    }

    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
        => await Repository.Statistics.GetStatisticsAsync();
}
