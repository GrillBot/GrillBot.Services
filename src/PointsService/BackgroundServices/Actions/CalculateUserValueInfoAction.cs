using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.BackgroundServices.Actions;

public class CalculateUserValueInfoAction : PostProcessActionBase
{
    public CalculateUserValueInfoAction(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(User user)
    {
        var baseQuery = Context.Transactions.AsNoTracking()
            .Where(o => o.GuildId == user.GuildId && o.UserId == user.Id);

        var yearBack = DateTime.UtcNow.AddYears(-1);
        var trackedUser = await ReadUserAsTracked(user);

        trackedUser.ActivePoints = await baseQuery.Where(o => o.CreatedAt >= yearBack && o.MergedCount == 0).SumAsync(o => o.Value);
        trackedUser.MergedPoints = await baseQuery.Where(o => o.MergedCount > 0).SumAsync(o => o.Value);
        trackedUser.ExpiredPoints = await baseQuery.Where(o => o.CreatedAt < yearBack && o.MergedCount == 0).SumAsync(o => o.Value);
        await Context.SaveChangesAsync();
    }

    private async Task<User> ReadUserAsTracked(User user)
    {
        return await Context.Users
            .FirstAsync(o => o.GuildId == user.GuildId && o.Id == user.Id);
    }
}
