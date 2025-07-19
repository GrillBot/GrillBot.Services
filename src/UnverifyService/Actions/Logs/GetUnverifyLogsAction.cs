using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Models.Pagination;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Models.Request.Logs;

namespace UnverifyService.Actions.Logs;

public partial class GetUnverifyLogsAction(IServiceProvider serviceProvider) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<UnverifyLogListRequest>(0);
        var query = CreateQuery(request);
        var pagedData = await ContextHelper.ReadEntitiesWithPaginationAsync(query, request.Pagination, CancellationToken);
        var result = await PaginatedResponse<Models.Response.Logs.UnverifyLogItem>.CopyAndMapAsync(pagedData, MapItemAsync);

        return ApiResult.Ok(result);
    }

    private IQueryable<UnverifyLogItem> CreateQuery(UnverifyLogListRequest request)
    {
        var query = DbContext.LogItems.AsNoTracking();

        if (request.Operation is not null)
            query = query.Where(x => x.OperationType == request.Operation);

        if (!string.IsNullOrEmpty(request.FromUserId))
        {
            var userId = request.FromUserId.ToUlong();
            query = query.Where(x => x.FromUserId == userId);
        }

        if (!string.IsNullOrEmpty(request.ToUserId))
        {
            var userId = request.ToUserId.ToUlong();
            query = query.Where(x => x.ToUserId == userId);
        }

        if (request.CreatedFrom is not null)
            query = query.Where(x => x.CreatedAt >= request.CreatedFrom);

        if (request.CreatedTo is not null)
            query = query.Where(x => x.CreatedAt <= request.CreatedTo);

        if (!string.IsNullOrEmpty(request.GuildId))
        {
            var guildId = request.GuildId.ToUlong();
            query = query.Where(x => x.GuildId == guildId);
        }

        if (request.ParentLogItemId is not null)
            query = query.Where(x => x.ParentLogItemId == request.ParentLogItemId);

        var sortExpression = request.Sort.OrderBy switch
        {
            "Operation" => [x => x.OperationType, x => x.CreatedAt],
            _ => new Expression<Func<UnverifyLogItem, object>>[] { x => x.CreatedAt }
        };

        return query.WithSorting(sortExpression, request.Sort.Descending);
    }
}
