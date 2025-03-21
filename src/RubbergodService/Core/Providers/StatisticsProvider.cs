using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;
using RubbergodService.Core.Entity;

namespace RubbergodService.Core.Providers;

public class StatisticsProvider(RubbergodServiceContext _dbContext) : IStatisticsProvider
{
    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return new Dictionary<string, long>
        {
            { nameof(_dbContext.Karma), await _dbContext.Karma.LongCountAsync() }
        };
    }
}
