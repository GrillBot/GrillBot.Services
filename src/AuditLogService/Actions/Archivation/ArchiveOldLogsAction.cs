using AuditLogService.Core.Entity;
using AuditLogService.Core.Options;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.Extensions.Options;

namespace AuditLogService.Actions.Archivation;

public partial class ArchiveOldLogsAction : ApiActionBase
{
    private AppOptions AppOptions { get; }
    private AuditLogServiceContext Context { get; }

    public ArchiveOldLogsAction(IOptionsSnapshot<AppOptions> options, AuditLogServiceContext context)
    {
        Context = context;
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
