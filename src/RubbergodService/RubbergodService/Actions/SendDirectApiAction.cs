using System.Text.Json;
using GrillBot.Core.Infrastructure.Actions;
using RubbergodService.Core.Helpers;
using RubbergodService.Core.Models;
using RubbergodService.DirectApi;

namespace RubbergodService.Actions;

public class SendDirectApiAction : ApiActionBase
{
    private DirectApiManager DirectApiManager { get; }

    public SendDirectApiAction(DirectApiManager directApiManager)
    {
        DirectApiManager = directApiManager;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var service = (string)Parameters[0]!;
        var command = (DirectApiCommand)Parameters[1]!;

        var response = await DirectApiManager.SendAsync(command, service);

        using var document = JsonDocument.Parse(response.Content);
        var json = await JsonHelper.SerializeJsonDocumentAsync(document);

        return new ApiResult(StatusCodes.Status200OK, json);
    }
}
