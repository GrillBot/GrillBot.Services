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
        var unverifies = await ComputeRowsAsync<UnverifyItem>(guildId);
        var warnings = await ComputeRowsAsync<MemberWarningItem>(guildId);
        var timeouts = await ComputeRowsAsync<TimeoutItem>(guildId);
        var result = unverifies + warnings + timeouts;

        return ApiResult.Ok(result);
    }

    private async Task<int> ComputeRowsAsync<TEntity>(string guildId) where TEntity : UserMeasureBase
        => await ContextHelper.ReadCountAsync(DbContext.Set<TEntity>().Where(o => o.GuildId == guildId));
}
