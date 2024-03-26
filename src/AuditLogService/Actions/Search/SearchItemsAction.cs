using AuditLogService.Core.Entity;
using AuditLogService.Models.Request.Search;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;

namespace AuditLogService.Actions.Search;

public partial class SearchItemsAction : ApiAction<AuditLogServiceContext>
{
    public SearchItemsAction(AuditLogServiceContext context, ICounterManager counterManager) : base(counterManager, context)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (SearchRequest)Parameters[0]!;

        request.UserIds = request.UserIds.Distinct().ToList();

        if (request.Ids is not null)
            request.Ids = request.Ids.Distinct().ToList();

        if (request.Ids is null || request.Ids.Count == 0)
            request.Ids = await SearchIdsFromAdvancedFilterAsync(request);

        var headers = await ReadLogHeadersAsync(request);
        var mapped = await MapAsync(headers);
        return ApiResult.Ok(mapped);
    }
}
