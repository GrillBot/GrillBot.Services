using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;

namespace UserMeasuresService.Actions.Info;

public class GetItemsCountOfGuild : ApiAction
{
    private UserMeasuresContext DbContext { get; }

    public GetItemsCountOfGuild(UserMeasuresContext dbContext, ICounterManager counterManager) : base(counterManager)
    {
        DbContext = dbContext;
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
        using (CreateCounter("Database"))
            return await DbContext.Unverifies.AsNoTracking().CountAsync(o => o.GuildId == guildId);
    }

    private async Task<int> ComputeWarningsAsync(string guildId)
    {
        using (CreateCounter("Database"))
            return await DbContext.MemberWarnings.AsNoTracking().CountAsync(o => o.GuildId == guildId);
    }
}
