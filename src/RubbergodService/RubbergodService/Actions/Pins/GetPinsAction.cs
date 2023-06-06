using GrillBot.Core.Infrastructure.Actions;
using Microsoft.AspNetCore.Mvc;
using RubbergodService.Core.Entity;
using RubbergodService.Core.Models;
using RubbergodService.Core.Repository;
using RubbergodService.DirectApi;

namespace RubbergodService.Actions.Pins;

public class GetPinsAction : ApiActionBase
{
    private RubbergodServiceRepository Repository { get; }
    private DirectApiManager DirectApiManager { get; }

    public GetPinsAction(RubbergodServiceRepository repository, DirectApiManager directApiManager)
    {
        Repository = repository;
        DirectApiManager = directApiManager;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (ulong)Parameters[0]!;
        var channelId = (ulong)Parameters[1]!;
        var markdown = (bool)Parameters[2]!;

        var contentType = markdown ? "text/markdown" : "application/json";
        var item = await TryFindFromCacheAsync(guildId, channelId, markdown);
        if (item is not null)
            return CreateResult(item, contentType);

        item = await GetFromRubbergodAsync(guildId, channelId, markdown);
        await Repository.AddAsync(item);
        await Repository.CommitAsync();

        return CreateResult(item, contentType);
    }

    private static ApiResult CreateResult(PinCacheItem item, string contentType)
        => new(StatusCodes.Status200OK, new FileContentResult(item.Data, contentType));

    private async Task<PinCacheItem?> TryFindFromCacheAsync(ulong guildId, ulong channelId, bool markdown)
    {
        var items = await ReadCacheAsync(guildId, channelId);
        return items.Find(o => Path.GetExtension(o.Filename) == (markdown ? ".md" : ".json"));
    }

    private async Task<PinCacheItem> GetFromRubbergodAsync(ulong guildId, ulong channelId, bool markdown)
    {
        var command = new DirectApiCommand { Method = "AutoPin" };
        command.Parameters.Add("command", "pin_get_all");
        command.Parameters.Add("type", markdown ? "markdown" : "json");
        command.Parameters.Add("channel", channelId.ToString());

        var response = await DirectApiManager.SendAsync(command, "Rubbergod");

        return new PinCacheItem
        {
            Data = response.Content,
            Filename = response.Filename,
            CreatedAtUtc = DateTime.UtcNow,
            ChannelId = channelId.ToString(),
            GuildId = guildId.ToString()
        };
    }

    /// <summary>
    /// Reads data from cache and removes expired data.
    /// </summary>
    private async Task<List<PinCacheItem>> ReadCacheAsync(ulong guildId, ulong channelId)
    {
        var result = new List<PinCacheItem>();
        var items = await Repository.PinCache.FindItemsByChannelAsync(guildId, channelId);
        var validFrom = DateTime.UtcNow.AddDays(-14);

        foreach (var item in items)
        {
            if (item.CreatedAtUtc <= validFrom)
                Repository.Remove(item);
            else
                result.Add(item);
        }

        if (items.Count != result.Count)
            await Repository.CommitAsync();
        return result;
    }
}
