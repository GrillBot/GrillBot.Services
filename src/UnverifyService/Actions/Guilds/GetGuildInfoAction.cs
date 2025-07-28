using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Response.Guilds;

namespace UnverifyService.Actions.Guilds;

public class GetGuildInfoAction(IServiceProvider serviceProvider) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<ulong>(0);

        var guildQuery = DbContext.Guilds.AsNoTracking()
            .Where(o => o.GuildId == guildId)
            .Select(o => new GuildInfo(
                o.MuteRoleId.ToString()
            ));

        var result = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildQuery, CancellationToken);

        return result is null ?
            ApiResult.NotFound() :
            ApiResult.Ok(result);
    }
}
