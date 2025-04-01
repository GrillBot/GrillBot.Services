using GrillBot.Core.Database;
using InviteService.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace InviteService.Core.Providers;

public class StatisticsProvider(InviteContext _dbContext) : IStatisticsProvider
{
    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return new Dictionary<string, long>
        {
            { nameof(_dbContext.Invites), await _dbContext.Invites.CountAsync() },
            { nameof(_dbContext.InviteUses), await _dbContext.InviteUses.CountAsync() }
        };
    }
}
