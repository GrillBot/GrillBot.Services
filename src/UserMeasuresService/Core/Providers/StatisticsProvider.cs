using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;

namespace UserMeasuresService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    private readonly UserMeasuresContext _dbContext;

    public StatisticsProvider(UserMeasuresContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        var result = new Dictionary<string, long>
        {
            { nameof(_dbContext.Unverifies), await _dbContext.Unverifies.CountAsync() },
            { nameof(_dbContext.MemberWarnings), await _dbContext.MemberWarnings.CountAsync() },
            { nameof(_dbContext.Timeouts), await _dbContext.Timeouts.CountAsync() }
        };

        return result.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
    }
}
