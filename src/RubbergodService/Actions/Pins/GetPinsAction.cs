using GrillBot.Core.Infrastructure.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RubbergodService.DirectApi;
using RubbergodService.Models;

namespace RubbergodService.Actions.Pins;

public class GetPinsAction(
    DirectApiManager _directApiManager,
    IDistributedCache _cache
) : ApiActionBase
{
    private static readonly DistributedCacheEntryOptions _cacheOptions = new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromDays(14));

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<ulong>(0);
        var channelId = GetParameter<ulong>(1);
        var markdown = GetParameter<bool>(2);

        var contentType = markdown ? "text/markdown" : "application/json";
        var cacheKey = $"RubbergodService/PinCacheItem({guildId}, {channelId}, {(markdown ? "md" : "json")})";
        var item = await _cache.GetAsync(cacheKey);
        if (item is not null)
            return CreateResult(item, contentType);

        item = await GetFromRubbergodAsync(channelId, markdown);
        await _cache.SetAsync(cacheKey, item, _cacheOptions);

        return CreateResult(item, contentType);
    }

    private static ApiResult CreateResult(byte[] data, string contentType)
        => ApiResult.Ok(new FileContentResult(data, contentType));

    private async Task<byte[]> GetFromRubbergodAsync(ulong channelId, bool markdown)
    {
        var command = new DirectApiCommand { Method = "AutoPin" };
        command.Parameters.Add("command", "pin_get_all");
        command.Parameters.Add("type", markdown ? "markdown" : "json");
        command.Parameters.Add("channel", channelId.ToString());

        var response = await _directApiManager.SendAsync(command, "Rubbergod");
        return response.Content;
    }
}
