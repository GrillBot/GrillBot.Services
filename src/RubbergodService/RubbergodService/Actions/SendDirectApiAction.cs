using System.Text.Json;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using RubbergodService.Core.Helpers;
using RubbergodService.Core.Models;
using RubbergodService.DirectApi;

namespace RubbergodService.Actions;

public class SendDirectApiAction : ApiActionBase
{
    private IConfiguration Configuration { get; }
    private DirectApiClient Client { get; }
    private ICounterManager CounterManager { get; }

    public SendDirectApiAction(IConfiguration configuration, DirectApiClient client, ICounterManager counterManager)
    {
        Configuration = configuration.GetRequiredSection("DirectApi");
        Client = client;
        CounterManager = counterManager;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var service = (string)Parameters[0]!;
        var command = (DirectApiCommand)Parameters[1]!;

        var configuration = Configuration.GetRequiredSection(service);
        var channelId = configuration.GetValue<ulong>("ChannelId");
        var timeout = configuration.GetValue<int>("Timeout");
        var timeoutChecks = configuration.GetValue<int>("Checks");

        using (CounterManager.Create("DirectAPI"))
        {
            using var jsonCommand = JsonSerializer.SerializeToDocument(command);

            var response = await Client.SendAsync(channelId, jsonCommand, timeout, timeoutChecks);
            using var document = JsonDocument.Parse(response);

            var json = await JsonHelper.SerializeJsonDocumentAsync(document);
            return new ApiResult(StatusCodes.Status200OK, json);
        }
    }
}
