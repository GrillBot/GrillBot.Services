using System.Text.Json;
using GrillBot.Core.Managers.Performance;
using RubbergodService.DirectApi.Models;
using RubbergodService.Models;

namespace RubbergodService.DirectApi;

public class DirectApiManager
{
    private IConfiguration Configuration { get; }
    private DirectApiClient DirectApiClient { get; }
    private ICounterManager CounterManager { get; }

    public DirectApiManager(IConfiguration configuration, DirectApiClient directApiClient, ICounterManager counterManager)
    {
        Configuration = configuration;
        DirectApiClient = directApiClient;
        CounterManager = counterManager;
    }

    public async Task<ApiResponse> SendAsync(DirectApiCommand command, string service)
    {
        var configuration = Configuration.GetRequiredSection($"DirectAPI:{service}");
        var channelId = configuration.GetValue<ulong>("ChannelId");
        var timeout = configuration.GetValue<int>("Timeout");
        var timeoutChecks = configuration.GetValue<int>("Checks");

        using var jsonDocument = JsonSerializer.SerializeToDocument(command);
        using (CounterManager.Create("DirectAPI"))
        {
            return await DirectApiClient.SendAsync(channelId, jsonDocument, timeout, timeoutChecks);
        }
    }
}
