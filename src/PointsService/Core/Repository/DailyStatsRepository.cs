using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models;

namespace PointsService.Core.Repository;

public class DailyStatsRepository : SubRepositoryBase<PointsServiceContext>
{
    public DailyStatsRepository(PointsServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
    }

    private IQueryable<DailyStat> GetBaseQuery(string guildId, bool disableTracking = false)
    {
        var query = Context.DailyStats
            .Where(o => o.GuildId == guildId);
        if (disableTracking)
            query = query.AsNoTracking();
        return query;
    }

    public async Task<List<DailyStat>> FindAllStatsForUserAsync(string guildId, string userId)
    {
        using (CreateCounter())
        {
            return await GetBaseQuery(guildId)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }
    }

    public async Task<List<PointsChartItem>> ComputeChartAsync(DateOnly dayFrom, DateOnly dayTo, string? guildId, string? userId)
    {
        using (CreateCounter())
        {
            var query = Context.DailyStats.AsNoTracking()
                .Where(o => o.Date >= dayFrom && o.Date <= dayTo);

            if (!string.IsNullOrEmpty(guildId))
                query = query.Where(o => o.GuildId == guildId);
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(o => o.UserId == userId);

            var groupedQuery = query
                .GroupBy(o => o.Date)
                .Select(o => new PointsChartItem
                {
                    MessagePoints = o.Sum(x => x.MessagePoints),
                    ReactionPoints = o.Sum(x => x.ReactionPoints),
                    Day = o.Key
                }).OrderBy(o => o.Day);

            return await groupedQuery.ToListAsync();
        }
    }
}
