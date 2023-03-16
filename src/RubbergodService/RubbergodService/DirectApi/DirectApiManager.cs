using System.Text.Json;
using GrillBot.Core.Managers.Performance;
using RubbergodService.Core.Models;

namespace RubbergodService.DirectApi;

public class DirectApiManager
{
    private IConfiguration Configuration { get; }
    private DirectApiClient Client { get; }
    private ICounterManager CounterManager { get; }

    public DirectApiManager(IConfiguration configuration, DirectApiClient client, ICounterManager counterManager)
    {
        Configuration = configuration.GetRequiredSection("DirectApi");
        Client = client;
        CounterManager = counterManager;
    }

    public async Task<JsonDocument> SendAsync(string service, DirectApiCommand command)
    {
        var configuration = Configuration.GetRequiredSection(service);
        var channelId = configuration.GetValue<ulong>("ChannelId");
        var timeout = configuration.GetValue<int>("Timeout");
        var timeoutChecks = configuration.GetValue<int>("Checks");

        using (CounterManager.Create("DirectAPI"))
        {
            using var jsonCommand = JsonSerializer.SerializeToDocument(command);

            var response = await Client.SendAsync(channelId, jsonCommand, timeout, timeoutChecks);
            return JsonDocument.Parse(response);
        }
    }
}
