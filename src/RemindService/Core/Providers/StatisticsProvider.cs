using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;
using RemindService.Core.Entity;

namespace RemindService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    private readonly RemindServiceContext _dbContext;

    public StatisticsProvider(RemindServiceContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return new Dictionary<string, long>
        {
            { nameof(_dbContext.RemindMessages), await _dbContext.RemindMessages.CountAsync() }
        };
    }
}
