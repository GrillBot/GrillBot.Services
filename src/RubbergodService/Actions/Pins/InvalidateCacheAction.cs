using GrillBot.Core.Infrastructure.Actions;
using RubbergodService.Core.Repository;

namespace RubbergodService.Actions.Pins;

public class InvalidateCacheAction : ApiActionBase
{
    private RubbergodServiceRepository Repository { get; }

    public InvalidateCacheAction(RubbergodServiceRepository repository)
    {
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (ulong)Parameters[0]!;
        var channelId = (ulong)Parameters[1]!;

        var channels = await Repository.PinCache.FindItemsByChannelAsync(guildId, channelId);

        Repository.RemoveCollection(channels);
        await Repository.CommitAsync();

        return ApiResult.Ok();
    }
}
