using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Core.Entity;

namespace UserManagementService.Core.Providers;

public class StatisticsProvider(UserManagementContext _dbContext) : IStatisticsProvider
{
    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        var result = new Dictionary<string, long>
        {
            { nameof(UserManagementContext.GuildUsers), await _dbContext.GuildUsers.LongCountAsync() },
            { nameof(UserManagementContext.Nicknames), await _dbContext.Nicknames.LongCountAsync() }
        };

        return result.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
    }
}
