using AuditLogService.Core.Entity;
using AuditLogService.Core.Options;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.Extensions.Options;

namespace AuditLogService.Actions.Archivation;

public partial class CreateArchivationDataAction : ApiAction<AuditLogServiceContext>
{
    private AppOptions AppOptions { get; }

    public CreateArchivationDataAction(IOptionsSnapshot<AppOptions> options, AuditLogServiceContext context, ICounterManager counterManager)
        : base(counterManager, context)
    {
        AppOptions = options.Get(null);
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var expirationDate = DateTime.UtcNow.AddMonths(-AppOptions.ExpirationMonths);

        if (!await ExistsItemsToArchiveAsync(expirationDate))
            return new ApiResult(StatusCodes.Status204NoContent);

        var items = await ReadItemsToArchiveAsync(expirationDate);
        var result = CreateArchive(items);

        return ApiResult.Ok(result);
    }
}
