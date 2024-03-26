using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using UserMeasuresService.Core.Entity;

namespace UserMeasuresService.Actions.Info;

public class GetItemsCountOfGuild : ApiAction<UserMeasuresContext>
{
    public GetItemsCountOfGuild(UserMeasuresContext dbContext, ICounterManager counterManager) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var unverifies = await ComputeUnverifiesAsync(guildId);
        var warnings = await ComputeWarningsAsync(guildId);
        var result = unverifies + warnings;

        return ApiResult.Ok(result);
    }

    private async Task<int> ComputeUnverifiesAsync(string guildId)
        => await ContextHelper.ReadCountAsync(DbContext.Unverifies.Where(o => o.GuildId == guildId));

    private async Task<int> ComputeWarningsAsync(string guildId)
        => await ContextHelper.ReadCountAsync(DbContext.MemberWarnings.Where(o => o.GuildId == guildId));
}
