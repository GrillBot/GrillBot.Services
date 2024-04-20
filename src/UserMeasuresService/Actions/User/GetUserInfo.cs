using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Models.User;

namespace UserMeasuresService.Actions.User;

public class GetUserInfo : ApiAction<UserMeasuresContext>
{
    public GetUserInfo(UserMeasuresContext dbContext, ICounterManager counterManager) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var userId = (string)Parameters[1]!;

        var result = new UserInfo
        {
            UnverifyCount = await ComputeCountAsync<UnverifyItem>(guildId, userId),
            WarningCount = await ComputeCountAsync<MemberWarningItem>(guildId, userId),
            TimeoutCount = await ComputeCountAsync<TimeoutItem>(guildId, userId)
        };

        return ApiResult.Ok(result);
    }

    public async Task<int> ComputeCountAsync<TEntity>(string guildId, string userId) where TEntity : UserMeasureBase
        => await ContextHelper.ReadCountAsync(DbContext.Set<TEntity>().Where(o => o.GuildId == guildId && o.UserId == userId));
}
