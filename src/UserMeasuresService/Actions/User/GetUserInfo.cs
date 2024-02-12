using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Models.User;

namespace UserMeasuresService.Actions.User;

public class GetUserInfo : ApiActionBase
{
    private UserMeasuresContext DbContext { get; }
    private ICounterManager CounterManager { get; }

    public GetUserInfo(UserMeasuresContext dbContext, ICounterManager counterManager)
    {
        DbContext = dbContext;
        CounterManager = counterManager;
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
    {
        using (CounterManager.Create("Api.Info.GetUserInfo.Database"))
            return await DbContext.Unverifies.AsNoTracking().CountAsync(o => o.GuildId == guildId && o.UserId == userId);
    }

    private async Task<int> ComputeWarningsAsync(string guildId, string userId)
    {
        using (CounterManager.Create("Api.Info.GetUserInfo.Database"))
            return await DbContext.MemberWarnings.AsNoTracking().CountAsync(o => o.GuildId == guildId && o.UserId == userId);
    }
}
