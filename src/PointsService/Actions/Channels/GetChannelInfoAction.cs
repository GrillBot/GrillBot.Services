using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models.Channels;

namespace PointsService.Actions.Channels;

public class GetChannelInfoAction(ICounterManager counterManager, PointsServiceContext dbContext) : ApiAction<PointsServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var channelId = (string)Parameters[1]!;
        var result = new ChannelInfo();

        await SetChannelInfoAsync(guildId, channelId, result);

        return ApiResult.Ok(result);
    }

    private async Task SetChannelInfoAsync(string guildId, string channelId, ChannelInfo result)
    {
        var channelQuery = DbContext.Channels.AsNoTracking().Where(o => o.GuildId == guildId && o.Id == channelId);
        var channel = await ContextHelper.ReadFirstOrDefaultEntityAsync(channelQuery);
        if (channel is null)
            return;

        result.PointsDeactivated = channel.PointsDisabled;
    }
}
