using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Redis.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using RubbergodService.DirectApi;
using RubbergodService.Models;
using System.Text;
using System.Text.Json;

namespace RubbergodService.Actions.Help;

public class GetSlashCommandsAction(
    DirectApiManager _directApiManager,
    IDistributedCache _cache
) : ApiActionBase
{
    private const string CacheKey = "RubbergodService/HelpSlashCommands";

    public override async Task<ApiResult> ProcessAsync()
    {
        var data = await _cache.GetAsync<Dictionary<string, Cog>>(CacheKey);
        if (data is not null)
            return ApiResult.Ok(data);

        data = await ReadFromRubbergodAsync();
        await _cache.SetAsync(CacheKey, data, TimeSpan.FromDays(7));
        return ApiResult.Ok(data);
    }

    private async Task<Dictionary<string, Cog>> ReadFromRubbergodAsync()
    {
        var command = new DirectApiCommand { Method = "Help" };
        command.Parameters.Add("command", "slash_commands");

        var apiResponse = await _directApiManager.SendAsync(command, "Rubbergod");
        var jsonData = Encoding.UTF8.GetString(apiResponse.Content);
        return JsonSerializer.Deserialize<Dictionary<string, Cog>>(jsonData)!;
    }
}
