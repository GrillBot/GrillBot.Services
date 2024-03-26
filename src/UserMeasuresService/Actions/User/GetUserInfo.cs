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
            UnverifyCount = await ComputeUnverifiesAsync(guildId, userId),
            WarningCount = await ComputeWarningsAsync(guildId, userId)
        };

        return ApiResult.Ok(result);
    }

    private async Task<int> ComputeUnverifiesAsync(string guildId, string userId)
        => await ContextHelper.ReadCountAsync(DbContext.Unverifies.Where(o => o.GuildId == guildId && o.UserId == userId));

    private async Task<int> ComputeWarningsAsync(string guildId, string userId)
        => await ContextHelper.ReadCountAsync(DbContext.MemberWarnings.Where(o => o.GuildId == guildId && o.UserId == userId));
}
