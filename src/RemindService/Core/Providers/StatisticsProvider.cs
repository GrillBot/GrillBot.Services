using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;
using RemindService.Core.Entity;

namespace RemindService.Core.Providers;

public class StatisticsProvider(RemindServiceContext dbContext) : IStatisticsProvider
{
    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return new Dictionary<string, long>
        {
            { nameof(dbContext.RemindMessages), await dbContext.RemindMessages.CountAsync() }
        };
    }
}
