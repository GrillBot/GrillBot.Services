using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Handlers.UserRecalculation;

public partial class UserRecalculationHandler
{
    private async Task ComputeUserInfoAsync(User user)
    {
        var yearBack = DateTime.UtcNow.AddYears(-1);
        var transactionsQuery = DbContext.Transactions.AsNoTracking()
            .Where(o => o.GuildId == user.GuildId && o.UserId == user.Id);

        using (CreateCounter("Database"))
        {
            user.ActivePoints = await transactionsQuery.Where(o => o.CreatedAt >= yearBack && o.MergedCount == 0).SumAsync(o => o.Value);
            user.MergedPoints = await transactionsQuery.Where(o => o.MergedCount > 0).SumAsync(o => o.Value);
            user.ExpiredPoints = await transactionsQuery.Where(o => o.CreatedAt < yearBack && o.MergedCount == 0).SumAsync(o => o.Value);
        }
    }
}
