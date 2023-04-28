using PointsService.Core.Entity;
using PointsService.Core.Repository;

namespace PointsService.BackgroundServices.PostProcessAction;

public class CalculateDailyStatsAction : PostProcessActionBase
{
    public CalculateDailyStatsAction(PointsServiceRepository repository) : base(repository)
    {
    }

    public override async Task ProcessAsync()
    {
        var request = GetParameter<PostProcessRequest>();

        if (request is null)
        {
            var user = GetParameter<User>();
            if (user is not null)
                await ComputeFullStatsAsync(user);
        }
        else
        {
            await ComputeStatsForDayAsync(request);
        }

        await Repository.CommitAsync();
    }

    private async Task ComputeFullStatsAsync(User user)
    {
        var fullStats = await Repository.Transaction.ComputeAllDaysStats(user.GuildId, user.Id);
        if (fullStats.Count == 0)
            return;

        var allStats = await Repository.DailyStats.FindAllStatsForUserAsync(user.GuildId, user.Id);

        foreach (var computedData in fullStats)
        {
            var stats = allStats.Find(o => o.Date == computedData.day);
            if (stats is null)
            {
                stats = new DailyStat
                {
                    GuildId = user.GuildId,
                    UserId = user.Id,
                    Date = computedData.day
                };

                await Repository.AddAsync(stats);
            }

            stats.MessagePoints = computedData.messagePoints;
            stats.ReactionPoints = computedData.reactionPoints;
        }

        await Repository.CommitAsync();
    }

    private async Task ComputeStatsForDayAsync(PostProcessRequest request)
    {
        var day = DateOnly.FromDateTime(request.Transaction.CreatedAt);
        var dailyStats = await Repository.Transaction.ComputeStatsForDay(request.Transaction.GuildId, request.Transaction.UserId, day);
        var current = await Repository.DailyStats.FindStatsForDayAsync(request.Transaction.GuildId, request.Transaction.UserId, day);

        if (current is null)
        {
            current = new DailyStat
            {
                GuildId = request.Transaction.GuildId,
                UserId = request.Transaction.UserId,
                Date = day
            };

            await Repository.AddAsync(current);
        }

        current.MessagePoints = dailyStats.messagePoints;
        current.ReactionPoints = dailyStats.reactionPoints;
    }
}
