using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.Extensions.Options;
using UnverifyService.Core.Entity;
using UnverifyService.Options;

namespace UnverifyService.Actions.Archivation;

public partial class CreateArchivationDataAction(
    IServiceProvider serviceProvider,
    IOptions<AppOptions> _options
) : ApiAction<UnverifyContext>(serviceProvider)
{
    private DateTime ExpirationDate =>
        DateTime.UtcNow.Add(-_options.Value.Archivation.ExpirationMilestone);

    public override async Task<ApiResult> ProcessAsync()
    {
        if (!await ExistsItemtoArchiveAsync())
            return new ApiResult(StatusCodes.Status204NoContent);

        var items = await ReadItemsToArchiveAsync();
        var archive = CreateArchive(items);

        return ApiResult.Ok(archive);
    }
}
