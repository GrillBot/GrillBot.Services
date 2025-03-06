using GrillBot.Core.Caching;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.Extensions.Caching.Distributed;
using RubbergodService.DirectApi;
using RubbergodService.Models;
using System.Text;
using System.Text.Json;

namespace RubbergodService.Actions.Help;

public class GetSlashCommandsAction : ApiActionBase
{
    private const string CacheKey = "RubbergodService/HelpSlashCommands";

    private DirectApiManager DirectApiManager { get; }

    private readonly IDistributedCache _cache;

    public GetSlashCommandsAction(DirectApiManager directApiManager, IDistributedCache cache)
    {
        DirectApiManager = directApiManager;
        _cache = cache;
    }

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

        var apiResponse = await DirectApiManager.SendAsync(command, "Rubbergod");
        var jsonData = Encoding.UTF8.GetString(apiResponse.Content);
        return JsonSerializer.Deserialize<Dictionary<string, Cog>>(jsonData)!;
    }
}
