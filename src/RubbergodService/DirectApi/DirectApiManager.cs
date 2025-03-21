using System.Text.Json;
using GrillBot.Core.Managers.Performance;
using RubbergodService.DirectApi.Models;
using RubbergodService.Models;

namespace RubbergodService.DirectApi;

public class DirectApiManager(IConfiguration _configuration, DirectApiClient _directApiClient, ICounterManager _counterManager)
{
    public async Task<ApiResponse> SendAsync(DirectApiCommand command, string service)
    {
        var configuration = _configuration.GetRequiredSection($"DirectAPI:{service}");
        var channelId = configuration.GetValue<ulong>("ChannelId");
        var timeout = configuration.GetValue<int>("Timeout");
        var timeoutChecks = configuration.GetValue<int>("Checks");

        using var jsonDocument = JsonSerializer.SerializeToDocument(command);
        using (_counterManager.Create("DirectAPI"))
        {
            return await _directApiClient.SendAsync(channelId, jsonDocument, timeout, timeoutChecks);
        }
    }
}
