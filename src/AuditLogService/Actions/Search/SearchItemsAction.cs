using AuditLogService.Core.Entity;
using AuditLogService.Models.Request.Search;
using GrillBot.Core.Infrastructure.Actions;

namespace AuditLogService.Actions.Search;

public partial class SearchItemsAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public SearchItemsAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (SearchRequest)Parameters[0]!;

        request.UserIds = request.UserIds.Distinct().ToList();

        if (request.Ids is not null)
            request.Ids = request.Ids.Distinct().ToList();

        if (request.Ids is null || request.Ids.Count == 0)
            request.Ids = await SearchIdsFromAdvancedFilterAsync(request);

        var headers = await ReadLogHeaders(request);
        var mapped = await MapAsync(headers);
        return ApiResult.FromSuccess(mapped);
    }
}
