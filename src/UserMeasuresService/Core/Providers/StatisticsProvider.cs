using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;

namespace UserMeasuresService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    private UserMeasuresContext DbContext { get; }

    public StatisticsProvider(UserMeasuresContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        var result = new Dictionary<string, long>
        {
            { nameof(DbContext.Unverifies), await DbContext.Unverifies.CountAsync() },
            { nameof(DbContext.MemberWarnings), await DbContext.MemberWarnings.CountAsync() }
        };

        return result.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
    }
}
