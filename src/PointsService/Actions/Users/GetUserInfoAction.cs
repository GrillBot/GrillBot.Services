using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models.Users;

namespace PointsService.Actions.Users;

public class GetUserInfoAction(
    ICounterManager counterManager,
    PointsServiceContext dbContext
) : ApiAction<PointsServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var userId = (string)Parameters[1]!;
        var result = new UserInfo();

        await SetUserInfoAsync(guildId, userId, result);

        if (!result.NoActivity)
            await SetPointsStatusAsync(guildId, userId, result);

        return ApiResult.Ok(result);
    }

    private async Task SetUserInfoAsync(string guildId, string userId, UserInfo info)
    {
        var userQuery = DbContext.Users.AsNoTracking().Where(o => o.GuildId == guildId && o.Id == userId);
        var user = await ContextHelper.ReadFirstOrDefaultEntityAsync(userQuery);
        if (user is null)
        {
            info.NoActivity = true;
            info.PointsDisabled = false;
            info.PointsPostion = null;
            return;
        }

        info.NoActivity = user.ActivePoints + user.ExpiredPoints + user.MergedPoints == 0;
        info.PointsDisabled = user.PointsDisabled;
        info.PointsPostion = user.PointsPosition;
    }

    private async Task SetPointsStatusAsync(string guildId, string userId, UserInfo info)
    {
        var leaderboardQuery = DbContext.Leaderboard.AsNoTracking().Where(o => o.GuildId == guildId && o.UserId == userId);
        var leaderboardItem = await ContextHelper.ReadFirstOrDefaultEntityAsync(leaderboardQuery);
        if (leaderboardItem is null)
            return;

        info.Status = new Models.PointsStatus
        {
            MonthBack = leaderboardItem.MonthBack,
            Today = leaderboardItem.Today,
            Total = leaderboardItem.Total,
            YearBack = leaderboardItem.YearBack
        };
    }
}
