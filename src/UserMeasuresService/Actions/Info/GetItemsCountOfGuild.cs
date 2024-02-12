using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;

namespace UserMeasuresService.Actions.Info;

public class GetItemsCountOfGuild : ApiActionBase
{
    private UserMeasuresContext DbContext { get; }
    private ICounterManager CounterManager { get; }

    public GetItemsCountOfGuild(UserMeasuresContext dbContext, ICounterManager counterManager)
    {
        DbContext = dbContext;
        CounterManager = counterManager;
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
    {
        using (CounterManager.Create("Api.Info.GetItemsCountOfGuild.Database"))
            return await DbContext.Unverifies.AsNoTracking().CountAsync(o => o.GuildId == guildId);
    }

    private async Task<int> ComputeWarningsAsync(string guildId)
    {
        using (CounterManager.Create("Api.Info.GetItemsCountOfGuild.Database"))
            return await DbContext.MemberWarnings.AsNoTracking().CountAsync(o => o.GuildId == guildId);
    }
}
