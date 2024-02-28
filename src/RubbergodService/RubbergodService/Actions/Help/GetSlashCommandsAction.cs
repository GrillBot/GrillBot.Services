using GrillBot.Core.Infrastructure.Actions;
using Microsoft.Extensions.Caching.Memory;
using RubbergodService.Core.Models;
using RubbergodService.DirectApi;
using System.Text;
using System.Text.Json;

namespace RubbergodService.Actions.Help;

public class GetSlashCommandsAction : ApiActionBase
{
    private const string CacheKey = "HelpSlashCommands";

    private DirectApiManager DirectApiManager { get; }
    private IMemoryCache MemoryCache { get; }

    public GetSlashCommandsAction(IMemoryCache memoryCache, DirectApiManager directApiManager)
    {
        DirectApiManager = directApiManager;
        MemoryCache = memoryCache;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var data = ReadFromCache();
        if (data is not null)
            return ApiResult.Ok(data);

        data = await ReadFromRubbergodAsync();
        MemoryCache.Set(CacheKey, data, DateTimeOffset.Now.AddDays(7));
        return ApiResult.Ok(data);
    }

    private Dictionary<string, Cog>? ReadFromCache()
        => MemoryCache.TryGetValue<Dictionary<string, Cog>>(CacheKey, out var commands) ? commands : null;

    private async Task<Dictionary<string, Cog>> ReadFromRubbergodAsync()
    {
        var command = new DirectApiCommand { Method = "Help" };
        command.Parameters.Add("command", "slash_commands");

        var apiResponse = await DirectApiManager.SendAsync(command, "Rubbergod");
        var jsonData = Encoding.UTF8.GetString(apiResponse.Content);
        return JsonSerializer.Deserialize<Dictionary<string, Cog>>(jsonData)!;
    }
}
