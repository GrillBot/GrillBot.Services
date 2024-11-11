using AuditLogService.Core.Entity;
using AuditLogService.Core.Options;
using AuditLogService.Models.Response.Info;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuditLogService.Actions.Info;

public class GetStatusInfoAction : ApiAction<AuditLogServiceContext>
{
    private AppOptions AppOptions { get; }

    public GetStatusInfoAction(AuditLogServiceContext context, IOptions<AppOptions> options, ICounterManager counterManager) : base(counterManager, context)
    {
        AppOptions = options.Value;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var result = new StatusInfo
        {
            ItemsToArchive = await ReadItemsToArchiveCountAsync()
        };

        return ApiResult.Ok(result);
    }

    private async Task<int> ReadItemsToArchiveCountAsync()
    {
        var expirationDate = DateTime.UtcNow.AddMonths(-AppOptions.ExpirationMonths);
        var query = DbContext.LogItems.Where(o => o.CreatedAt <= expirationDate || o.IsDeleted).AsNoTracking();

        return await ContextHelper.ReadCountAsync(query);
    }
}
