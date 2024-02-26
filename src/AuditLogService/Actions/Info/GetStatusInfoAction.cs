using AuditLogService.Core.Entity;
using AuditLogService.Core.Options;
using AuditLogService.Models.Response.Info;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuditLogService.Actions.Info;

public class GetStatusInfoAction : ApiAction
{
    private AuditLogServiceContext DbContext { get; }
    private AppOptions AppOptions { get; }

    public GetStatusInfoAction(AuditLogServiceContext context, IOptions<AppOptions> options, ICounterManager counterManager) : base(counterManager)
    {
        DbContext = context;
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

        using (CreateCounter("Database"))
            return await DbContext.LogItems.AsNoTracking().CountAsync(o => o.CreatedAt <= expirationDate);
    }
}
